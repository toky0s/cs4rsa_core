using cs4rsa_core.BaseClasses;
using cs4rsa_core.Dialogs.DialogResults;
using cs4rsa_core.Dialogs.MessageBoxService;
using cs4rsa_core.Messages.Publishers.Dialogs;
using MaterialDesignThemes.Wpf;
using CommunityToolkit.Mvvm.Messaging;
using System.Threading.Tasks;
using System.Windows;
using cs4rsa_core.Cs4rsaDatabase.Models;
using cs4rsa_core.Services.StudentCrawlerSvc.Crawlers;
using cs4rsa_core.Services.StudentCrawlerSvc.Crawlers.Interfaces;
using cs4rsa_core.Utils.Interfaces;
using System;
using System.IO;

namespace cs4rsa_core.Dialogs.Implements
{
    /// <summary>
    /// ViewModel của dialog nhập session id
    /// view model này là sử dụng chính DtuStudentInfoCrawler để cào thông tin sinh viên.
    /// Ngoài ra không có bất cứ viewmodel nào được sử dụng crawler này.
    /// </summary>
    public class SessionInputViewModel : ViewModelBase
    {
        private string _sessionId;
        public string SessionId
        {
            get => _sessionId;
            set
            {
                _sessionId = value;
                OnPropertyChanged();
            }
        }

        public IMessageBox MessageBox { get; set; }
        private readonly IDtuStudentInfoCrawler _dtuStudentInfoCrawler;
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;
        private readonly IFolderManager _folderManager;

        public SessionInputViewModel(
            IDtuStudentInfoCrawler dtuStudentInfoCrawler,
            ISnackbarMessageQueue snackbarMessageQueue,
            IFolderManager folderManager
        )
        {
            _dtuStudentInfoCrawler = dtuStudentInfoCrawler;
            _snackbarMessageQueue = snackbarMessageQueue;
            _folderManager = folderManager;
        }

        public async Task Find()
        {
            SpecialStringCrawler specialStringCrawlerV1 = new();
            SpecialStringCrawlerV2 specialStringCrawlerV2 = new();
            Task<string> specialStringV1 = specialStringCrawlerV1.GetSpecialString(_sessionId);
            Task<string> specialStringV2 = specialStringCrawlerV2.GetSpecialString(_sessionId);
            string[] specialStrings = await Task.WhenAll(specialStringV1, specialStringV2);
            if (specialStrings[0] is null && specialStrings[1] is null)
            {
                string message = "Hãy chắc chắn bạn đã đăng nhập vào MyDTU trước khi lấy Session ID, " +
                    "và đảm bảo lúc này server DTU không bảo trì. Hãy thử lại sau.";
                MessageBoxResult _ = MessageBox.ShowMessage(message,
                                        "Thông báo",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Exclamation);   
            }
            else
            {
                // Lấy thông tin sinh viên thành công.
                Student student = await _dtuStudentInfoCrawler.Crawl(specialStrings[0] ?? specialStrings[1]);
                if (student != null)
                {
                    string path = Path.Combine(AppContext.BaseDirectory, IFolderManager.FD_STUDENT_PROGRAMS, student.StudentId);
                    _folderManager.CreateFolderIfNotExists(path);
                }
                string message = $"Xin chào {student.Name}";
                _snackbarMessageQueue.Enqueue(message);
                StudentResult result = new() { Student = student };
                Messenger.Send(new SessionInputVmMsgs.ExitSearchAccountMsg(result));
            }
            CloseDialog();
        }
    }
}
