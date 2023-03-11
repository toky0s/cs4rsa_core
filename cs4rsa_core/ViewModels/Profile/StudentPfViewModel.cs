using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Cs4rsa.BaseClasses;
using Cs4rsa.Constants;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;
using Cs4rsa.Services.StudentCrawlerSvc.Crawlers.Interfaces;
using Cs4rsa.Services.StudentCrawlerSvc.Models;
using Cs4rsa.Utils;
using Cs4rsa.Utils.Interfaces;

using MaterialDesignThemes.Wpf;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Cs4rsa.ViewModels.Profile
{
    public partial class StudentPfViewModel : ViewModelBase
    {
        private readonly int Limit;
        public ObservableCollection<StudentModel> StudentModels { get; set; }
        public ObservableCollection<Student> SavedStudentModels { get; set; }

        /// <summary>
        /// Tìm kiếm mã sinh viên.
        /// </summary>
        [ObservableProperty]
        private string _searchStudentId;

        /// <summary>
        /// Số lượng item cho mỗi batch sẽ được tạo.
        /// </summary>
        [ObservableProperty]
        private int _batchSize;

        /// <summary>
        /// Thời gian đợi sau khi mỗi Chunk được tải xong.
        /// </summary>
        [ObservableProperty]
        private int _waitBySecond;

        /// <summary>
        /// Trang hiện tại
        /// </summary>
        [ObservableProperty]
        private int _currentPage;

        /// <summary>
        /// Tổng số trang
        /// </summary>
        [ObservableProperty]
        private long _totalPage;

        private readonly IDtuStudentInfoCrawler _studentCrawler;
        private readonly IOpenInBrowser _openInBrowser;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISnackbarMessageQueue _snackbarMsgQueue;

        public StudentPfViewModel(
            IDtuStudentInfoCrawler studentCrawler,
            IOpenInBrowser openInBrowser,
            IUnitOfWork unitOfWork,
            ISnackbarMessageQueue snackbarMsgQueue
        )
        {
            _studentCrawler = studentCrawler;
            _openInBrowser = openInBrowser;
            _unitOfWork = unitOfWork;
            _snackbarMsgQueue = snackbarMsgQueue;

            StudentModels = new();
            SavedStudentModels = new();
            BatchSize = 5;
            WaitBySecond = 3;
            CurrentPage = 1;
            Limit = 20;

            StudentModels.CollectionChanged += StudentModels_CollectionChanged;
        }

        private void StudentModels_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            DownloadCommand.NotifyCanExecuteChanged();
        }

        public async Task OnInit()
        {
            TotalPage = _unitOfWork.Students.CountPage(Limit);
            NextPageCommand.NotifyCanExecuteChanged();
            PreviousPageCommand.NotifyCanExecuteChanged();
            await LoadStudents();
        }

        /// <summary>
        /// Tải các sinh viên đã lưu trong DB.
        /// </summary>
        /// <returns>Task</returns>
        private async Task LoadStudents()
        {
            SavedStudentModels.Clear();
            IAsyncEnumerable<Student> students = _unitOfWork.Students.Get(CurrentPage, Limit);
            await foreach (Student student in students)
            {
                SavedStudentModels.Add(student);
            }
            NextPageCommand.NotifyCanExecuteChanged();
            PreviousPageCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(AllowConcurrentExecutions = true, CanExecute = "CanDownload")]
        private async Task OnDownload()
        {
            GetClipboardCommand.CanExecute(false);
            IEnumerable<Task[]> taskChunk = StudentModels
                .Where(st => st.IsSuccess == false)
                .Select(st => CreateDownloadTask(st.StudentId))
                .Chunk(BatchSize);

            foreach (Task[] tasks in taskChunk)
            {
                await Task.WhenAll(tasks);
                await Task.Delay(WaitBySecond * 1000);
            }
            DownloadCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = "CanGetClipboard")]
        private void OnGetClipboard()
        {
            string text = Clipboard.GetText();
            InitStudents(text);
            DownloadCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand]
        private void OnOpenInFolder(string studentId)
        {
            _openInBrowser.OpenFolderAndSelect(CredizText.PathStudentProfileImg(studentId));
            DownloadCommand.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// Xoá các file ảnh có dung lượng bằng 0 KB.
        /// </summary>
        [RelayCommand]
        private void OnCleanFolder()
        {
            DirectoryInfo imgFolder = new(IFolderManager.FdStudentImgs);
            IEnumerable<FileInfo> zeroLengthfiles = imgFolder.GetFiles().Where(f => f.Length == 0);
            foreach (FileInfo file in zeroLengthfiles)
            {
                file.Delete();
            }
            _snackbarMsgQueue.Enqueue(CredizText.Common002("Làm sạch thư mục chứa ảnh"));
        }

        /// <summary>
        /// Quay về trang trước.
        /// </summary>
        [RelayCommand(AllowConcurrentExecutions = true, CanExecute = "CanPreviousPage")]
        private async Task OnPreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                await LoadStudents();
            }
        }

        [RelayCommand(AllowConcurrentExecutions = true)]
        public async Task OnSearch()
        {
            string clearValue = SearchStudentId.Trim();
            if (string.IsNullOrEmpty(clearValue))
            {
                CurrentPage = 1;
                TotalPage = _unitOfWork.Students.CountPage(Limit);
                await LoadStudents();
            }
            else
            {
                int totalItem = await _unitOfWork.Students.CountByContainsId(clearValue);
                TotalPage = totalItem / Limit;
                CurrentPage = 1;
                await LoadStudents(clearValue, Limit, CurrentPage);
            }
            NextPageCommand.NotifyCanExecuteChanged();
            PreviousPageCommand.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// Tìm kiếm sinh viên.
        /// </summary>
        /// <param name="studentIdCriteria">Điều kiện tìm kiếm mã sinh viên.</param>
        /// <param name="pageSize">Số lượng phần tử mỗi trang.</param>
        /// <param name="page">Số trang.</param>
        /// <returns>Danh sách sinh viên.</returns>
        private async Task LoadStudents(string studentIdCriteria, int pageSize, int page)
        {
            SavedStudentModels.Clear();
            IAsyncEnumerable<Student> students = _unitOfWork.Students.GetStudentsByContainsId(studentIdCriteria, pageSize, page);
            await foreach (Student student in students)
            {
                SavedStudentModels.Add(student);
            }
        }

        private bool CanPreviousPage()
        {
            return CurrentPage > 1;
        }

        /// <summary>
        /// Tới trang sau.
        /// </summary>
        [RelayCommand(AllowConcurrentExecutions = true, CanExecute = "CanNextPage")]
        private async Task OnNextPage()
        {
            if (CurrentPage < TotalPage)
            {
                CurrentPage++;
                await LoadStudents();
            }
        }

        private bool CanNextPage()
        {
            return CurrentPage < TotalPage;
        }

        private bool CanDownload()
        {
            return StudentModels.Any(sm => sm.Downloaded == false);
        }

        private bool CanGetClipboard()
        {
            return StudentModels.All(sm => sm.IsDownloading == false);
        }

        private void InitStudents(string studentIds)
        {
            StudentModels.Clear();
            IEnumerable<string> studentCodes = StringHelper
                .SplitAndRemoveAllSpace(studentIds)
                .Where(dt => Regex.IsMatch(dt, "^[0-9]*$"));
            foreach (string studentId in studentCodes)
            {
                //1. Trường hợp đã có trong DB và hình ảnh đã tải đã có trong Folder.
                bool existsInFolder = File.Exists(CredizText.PathStudentProfileImg(studentId));
                if (existsInFolder)
                {
                    StudentModels.Add(new StudentModel
                    {
                        StudentId = studentId,
                        IsDownloading = false,
                        Downloaded = true,
                        IsSuccess = true
                    });
                }
                else
                {
                    StudentModels.Add(new StudentModel
                    {
                        StudentId = studentId,
                        IsDownloading = false,
                        Downloaded = false,
                        IsSuccess = false
                    });
                }
            }
        }

        public void AddStudentIdToDownload(string studentId)
        {
            if (string.IsNullOrEmpty(studentId)) return;

            if (StudentModels.Any(sm => sm.StudentId.Equals(studentId)))
            {
                // Move on top
                StudentModel sm = StudentModels
                    .First(sm => sm.StudentId.Equals(studentId));
                int idx = StudentModels.IndexOf(sm);
                StudentModels.Move(idx, 0);
            }
            else
            {
                StudentModel studentModel = new()
                {
                    StudentId = studentId,
                    IsSuccess = false,
                    IsDownloading = false,
                    Downloaded = false
                };
                StudentModels.Insert(0, studentModel);
            }
        }

        /// <summary>
        /// Tạo Task download ảnh hồ sơ sinh viên.
        /// </summary>
        /// <param name="studentId">Mã sinh viên</param>
        /// <returns>Task</returns>
        private async Task CreateDownloadTask(string studentId)
        {
            StudentModel item = StudentModels
                .Where(sm => sm.StudentId.Equals(studentId))
                .First();

            item.IsDownloading = true;
            item.Downloaded = false;
            item.IsSuccess = false;

            string result = await _studentCrawler.DownloadProfileImg(studentId);

            item.IsDownloading = false;
            item.Downloaded = true;
            item.IsSuccess = result != null && studentId.Equals(result);

            string imgPath = CredizText.PathStudentProfileImg(studentId);
            if (result == null)
            {
                if (File.Exists(imgPath))
                {
                    File.Delete(imgPath);
                }
            }
            else
            {
                item.ImgPath = imgPath;
                // 1. Lưu kết quả trong trường hợp tải ảnh thành công.
                await _unitOfWork.BeginTransAsync();
                try
                {
                    Student student = new()
                    {
                        StudentId = studentId,
                        AvatarImgPath = imgPath,
                    };
                    await _unitOfWork.Students.AddAsync(student);
                    await _unitOfWork.CompleteAsync();
                    await _unitOfWork.CommitAsync();

                    if (string.IsNullOrEmpty(SearchStudentId) || studentId.Contains(SearchStudentId))
                    {
                        SavedStudentModels.Add(student);
                    }
                }
                catch
                {
                    await _unitOfWork.RollbackAsync();
                }
            }
        }
    }
}
