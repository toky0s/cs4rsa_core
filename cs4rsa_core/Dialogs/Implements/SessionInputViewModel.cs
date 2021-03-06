using cs4rsa_core.Dialogs.DialogResults;
using cs4rsa_core.Dialogs.DialogServices;
using cs4rsa_core.Dialogs.MessageBoxService;
using cs4rsa_core.Messages;
using cs4rsa_core.ViewModels;

using Cs4rsaDatabaseService.Models;

using LightMessageBus;

using MaterialDesignThemes.Wpf;

using StudentCrawlerService.Crawlers;
using StudentCrawlerService.Crawlers.Interfaces;

using System.Threading.Tasks;
using System.Windows;

namespace cs4rsa_core.Dialogs.Implements
{
    /// <summary>
    /// ViewModel của dialog nhập session id
    /// view model này là sử dụng chính DtuStudentInfoCrawler để cào thông tin sinh viên.
    /// Ngoài ra không có bất cứ viewmodel nào được sử dụng crawler này.
    /// </summary>
    public class SessionInputViewModel : DialogViewModelBase<StudentResult>
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

        public SessionInputViewModel(
            IDtuStudentInfoCrawler dtuStudentInfoCrawler,
            ISnackbarMessageQueue snackbarMessageQueue
            )
        {
            _dtuStudentInfoCrawler = dtuStudentInfoCrawler;
            _snackbarMessageQueue = snackbarMessageQueue;
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
                (Application.Current.MainWindow.DataContext as MainWindowViewModel).CloseDialog();
            }
            else
            {
                Student student = await _dtuStudentInfoCrawler.Crawl(specialStrings[0] ?? specialStrings[1]);
                //await _firebase.PostUser(student.StudentId, student.SpecialString);
                string message = $"Xin chào {student.Name}";
                _snackbarMessageQueue.Enqueue(message);
                StudentResult result = new() { Student = student };
                MessageBus.Default.Publish(new ExitSessionInputMessage(result));
                (Application.Current.MainWindow.DataContext as MainWindowViewModel).CloseDialog();
            }
        }
    }
}
