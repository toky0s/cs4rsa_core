using cs4rsa_core.BaseClasses;
using cs4rsa_core.Commons;

using Cs4rsaDatabaseService.Interfaces;
using Cs4rsaDatabaseService.Models;

using HelperService.Interfaces;

using MaterialDesignThemes.Wpf;

using Microsoft.Toolkit.Mvvm.Input;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace cs4rsa_core.ViewModels
{
    public class StudentSearchViewModel : ViewModelBase
    {
        #region Static Fields
        private static readonly string DEFAULT_IMAGE_PATH = "/Images/default_avatar.png";

        private static readonly string DEFAULT_IMAGE_FOLDER_NAME = "StudentImages";
        #endregion

        #region Fields
        private static string CurrentImageFolderPath = "";
        #endregion

        #region Properties
        /// <summary>
        /// Hình ảnh sinh viên hiện tại
        /// </summary>
        private StudentImage _currentStudentImage;
        public StudentImage CurrentStudentImage
        {
            get { return _currentStudentImage; }
            set { _currentStudentImage = value; OnPropertyChanged(); }
        }

        private string _defaultImage;
        public string DefaultImage
        {
            get
            {
                if (_defaultImage == string.Empty || _defaultImage == null)
                {
                    return DEFAULT_IMAGE_PATH;
                }
                else
                {
                    return _defaultImage;
                }
            }
            set
            {
                _defaultImage = value;
                OnPropertyChanged();
            }
        }

        private string _code;
        public string Code
        {
            get => _code;
            set
            {
                _code = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<StudentImage> _studentImages;

        public ObservableCollection<StudentImage> StudentImages
        {
            get { return _studentImages; }
            set { _studentImages = value; }
        }

        #endregion

        #region DI
        private readonly IFolderManager _folderManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;
        #endregion

        #region Commands
        public AsyncRelayCommand SearchCommand { get; set; }
        #endregion

        public StudentSearchViewModel(
            IFolderManager folderManager,
            IUnitOfWork unitOfWork,
            ISnackbarMessageQueue snackbarMessageQueue
            )
        {
            _folderManager = folderManager;
            _unitOfWork = unitOfWork;
            _snackbarMessageQueue = snackbarMessageQueue;

            _studentImages = new();

            SearchCommand = new AsyncRelayCommand(DownloadImage);

            CurrentImageFolderPath = _folderManager.CreateFolderIfNotExists(DEFAULT_IMAGE_FOLDER_NAME);
        }


        /// <summary>
        /// Lấy ra danh sách hình ảnh sinh viên đã thu được.
        /// </summary>
        /// <returns></returns>
        public void GetStudentImages()
        {
            IEnumerable<StudentImage> studentImages = _unitOfWork.StudentImages.Get(si => si.FirstName);
            foreach (StudentImage studentImage in studentImages)
            {
                _studentImages.Add(studentImage);
            }
        }

        private async Task DownloadImage()
        {
            if (_code == string.Empty)
            {
                GetStudentImages();
                return;
            }
            StudentImage si = _unitOfWork.StudentImages.GetByStudentCode(_code);
            if (si != null)
            {
                if (File.Exists(si.Path))
                {
                    _studentImages.Clear();
                    _studentImages.Add(si);
                    return;
                }
            }
            StudentImage result = await GetStudentImageByCode(_code);
            if (result != null)
            {
                _studentImages.Add(result);
                string message = $"Đã lưu {result.Code} vào thư mục";
                _snackbarMessageQueue.Enqueue(message, "MỞ THƯ MỤC", OpenImageFolder);
            }
            else
            {
                _snackbarMessageQueue.Enqueue(Msg.SI__SAVING_PROCESS_ERROR);
            }
        }

        private async Task<StudentImage> GetStudentImageByCode(string code)
        {
            string endpoint = GetEndpointByCode(code);
            Uri uri = new(endpoint);
            string result = await DownloadImageAsync(CurrentImageFolderPath, code, uri);
            if (result == string.Empty)
            {
                _snackbarMessageQueue.Enqueue(Msg.SI__CODE_NOT_EXIST_OR_REMOVED);
            }

            StudentImage studentImage = new()
            {
                Code = code,
                Path = result,
                FirstName = "",
                LastName = "",
            };

            try
            {
                await _unitOfWork.BeginTransAsync();
                await _unitOfWork.StudentImages.AddAsync(studentImage);
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitAsync();
                return studentImage;
            }
            catch
            {
                return null;
            }
        }

        private static async Task<string> DownloadImageAsync(string directoryPath, string fileName, Uri uri)
        {
            try
            {
                using HttpClient httpClient = new();
                string fileExtension = ".jpg";
                string path = Path.Combine(directoryPath, $"{fileName}{fileExtension}");
                byte[] imageBytes = await httpClient.GetByteArrayAsync(uri);
                await ByteArrayToFile(path, imageBytes);
                return path;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"DownloadImageAsync Error {ex.Message}",
                    "Student Search",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return string.Empty;
            }
        }

        /// <summary>
        /// Lấy ra endpoint tải ảnh bằng mã sinh viên
        /// </summary>
        /// <param name="code">Mã sinh viên</param>
        /// <returns>Endpoint</returns>
        private static string GetEndpointByCode(string code)
        {
            return $"http://hfs1.duytan.edu.vn/Upload/dichvu/sv_{code}_01.jpg";
        }

        /// <summary>
        /// Mở thư mục chưa ảnh trong explorer
        /// </summary>
        private static void OpenImageFolder()
        {
            string path = $"{RootFolderPath}\\{DEFAULT_IMAGE_FOLDER_NAME}\\";
            OpenFolderWithExplorer(path);
        }

        private static async Task<bool> ByteArrayToFile(string fileName, byte[] byteArray)
        {
            try
            {
                using var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                await fs.WriteAsync(byteArray);
                fs.Close();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in process: {0}", ex);
                return false;
            }
        }
    }
}
