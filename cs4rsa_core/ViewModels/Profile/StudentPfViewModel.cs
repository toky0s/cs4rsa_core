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
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Cs4rsa.Cs4rsaDatabase.DataProviders;

namespace Cs4rsa.ViewModels.Profile
{
    public partial class StudentPfViewModel : ViewModelBase
    {
        /// <summary>
        /// Giới hạn số lượng hình ảnh
        /// được tải trong một trang.
        /// </summary>
        private readonly int _limit;
        /// <summary>
        /// Danh sách Student cần tải.
        /// </summary>
        public ObservableCollection<StudentModel> StudentModels { get; set; }
        /// <summary>
        /// Danh sách của Student đang được lưu trong DB.
        /// </summary>
        public ObservableCollection<Student> SavedStudentModels { get; set; }
        /// <summary>
        /// Danh sách Student được hiển thị trên giao diện.
        /// </summary>
        public ObservableCollection<Student> UiStudents { get; set; }

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

            StudentModels = new ObservableCollection<StudentModel>();
            SavedStudentModels = new ObservableCollection<Student>();
            UiStudents = new ObservableCollection<Student>();
            BatchSize = 5;
            WaitBySecond = 3;
            CurrentPage = 1;
            _limit = 10;

            StudentModels.CollectionChanged += StudentModels_CollectionChanged;
        }

        partial void OnSearchStudentIdChanged(string oldValue, string newValue)
        {
            Task.Run(() =>
            {
                if (string.IsNullOrWhiteSpace(newValue))
                {
                    Application.Current.Dispatcher.Invoke(StartPaging);
                    return;
                }
                IEnumerable<Student> students = SavedStudentModels
                    .Where(st => st.StudentId.Contains(newValue))
                    .ToArray();
                if (!students.Any()) return;
                Application.Current.Dispatcher.Invoke(() => UiStudents.Clear());
                foreach (Student student in students)
                {
                    Application.Current.Dispatcher.Invoke(() => UiStudents.Add(student));
                }
            });
        }

        private void StudentModels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            DownloadCommand.NotifyCanExecuteChanged();
        }

        public void OnInit()
        {
            LoadStudents();
            StartPaging();
            NextPageCommand.NotifyCanExecuteChanged();
            PreviousPageCommand.NotifyCanExecuteChanged();
        }

        private void StartPaging()
        {
            UiStudents.Clear();
            IEnumerable<Student> pagingStudents = SavedStudentModels
                .Skip(_limit * (CurrentPage - 1))
                .Take(_limit);
            foreach (Student student in pagingStudents)
            {
                UiStudents.Add(student);    
            }
            NextPageCommand.NotifyCanExecuteChanged();
            PreviousPageCommand.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// Tải tất cả các sinh viên đã lưu trong DB.
        /// </summary>
        /// <returns>Task</returns>
        private void LoadStudents()
        {
            SavedStudentModels.Clear();
            _unitOfWork.Students.GetAll().ForEach(st =>
            {
                SavedStudentModels.Add(st);
            });
            TotalPage = MathUtils.CountPage(SavedStudentModels.Count, _limit);
        }

        [RelayCommand]
        private void OnOpenContainFolder()
        {
            _openInBrowser.Open(IFolderManager.FdStudentImgs);
        }

        [RelayCommand(AllowConcurrentExecutions = true, CanExecute = "CanDownload")]
        private async Task OnDownload()
        {
            GetClipboardCommand.CanExecute(false);
            IEnumerable<IEnumerable<Task>> taskChunk = StudentModels
                .Where(st => st.IsSuccess == false)
                .Select(st => CreateDownloadTask(st.StudentId))
                .Chunk(BatchSize);

            // Thực thi tải ảnh theo batch
            foreach (IEnumerable<Task> tasks in taskChunk)
            {
                await Task.WhenAll(tasks);
                LoadStudents();
                StartPaging();
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
            IEnumerable<FileInfo> zeroLengthFiles = imgFolder
                .GetFiles()
                .Where(f => f.Length == 0);
            foreach (FileInfo file in zeroLengthFiles)
            {
                file.Delete();
            }
            _snackbarMsgQueue.Enqueue(CredizText.Common002("Xoá ảnh lỗi"));
        }

        /// <summary>
        /// Quay về trang trước.
        /// </summary>
        [RelayCommand(CanExecute = "CanPreviousPage")]
        private void OnPreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                StartPaging();
            }
        }

        private bool CanPreviousPage()
        {
            return CurrentPage > 1;
        }

        /// <summary>
        /// Tới trang sau.
        /// </summary>
        [RelayCommand(CanExecute = "CanNextPage")]
        private void OnNextPage()
        {
            if (CurrentPage < TotalPage)
            {
                CurrentPage++;
                StartPaging();
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

        /// <summary>
        /// Thêm mã sinh viên để download.
        /// </summary>
        /// <param name="studentId">Mã sinh viên.</param>
        public void AddStudentIdToDownload(string studentId)
        {
            if (string.IsNullOrWhiteSpace(studentId)) return;

            // Trường hợp trong cùng một phiên sử dụng ứng dụng
            // người dùng thực hiện tải cùng một mã hai lần.
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
            StudentModel item = StudentModels.First(sm => sm.StudentId.Equals(studentId));

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
                Student student = new()
                {
                    StudentId = studentId,
                    AvatarImgPath = imgPath,
                };
                
                if (_unitOfWork.Students.ExistsByStudentCode(studentId))
                {
                    Student s = _unitOfWork.Students.GetByStudentId(studentId);
                    s.AvatarImgPath = imgPath;
                    _unitOfWork.Students.Update(s);
                }
                else
                {
                    _unitOfWork.Students.Add(student);
                }

                // Nếu đang có sẵn trong danh sách thì thực hiện replace.
                if (SavedStudentModels.Any(st => st.StudentId.Equals(studentId)))
                {
                    for (int i = 0; i < SavedStudentModels.Count; i++)
                    {
                        if (SavedStudentModels[i].StudentId.Equals(studentId))
                        {
                            SavedStudentModels[i] = student;
                            break;
                        }
                    }
                }
                else
                {
                    SavedStudentModels.Add(student);
                }
            }
        }
    }
}
