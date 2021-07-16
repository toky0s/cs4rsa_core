using cs4rsa.BasicData;
using cs4rsa.Crawler;
using cs4rsa.Database;
using cs4rsa.Dialogs.DialogResults;
using cs4rsa.Dialogs.DialogService;
using cs4rsa.Dialogs.MessageBoxService;
using cs4rsa.Implements;
using System;
using System.ComponentModel;
using System.Windows;
using LightMessageBus;
using cs4rsa.Messages;

namespace cs4rsa.Dialogs.Implements
{
    /// <summary>
    /// ViewModel của dialog nhập session id
    /// view model này là sử dụng chính DtuStudentInfoCrawler để cào thông tin sinh viên.
    /// Ngoài ra không có bất cứ viewmodel nào được sử dụng crawler này.
    /// </summary>
    class SessionInputViewModel : DialogViewModelBase<StudentResult>
    {
        private string _sessionId;
        public string SessionId
        {
            get
            {
                return _sessionId;
            }
            set
            {
                _sessionId = value;
                RaisePropertyChanged();
            }
        }

        private int _progressValue;
        public int ProgressValue
        {
            get
            {
                return _progressValue;
            }
            set
            {
                _progressValue = value;
                RaisePropertyChanged();
            }
        }

        private IMessageBox _messageBox;
        private BackgroundWorker _backgroundWorker;

        public SessionInputViewModel(string sessionId, IMessageBox messageBox)
        {
            _sessionId = sessionId;
            _messageBox = messageBox;
            Find();
        }

        private void Find()
        {
            _backgroundWorker = new BackgroundWorker()
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            _backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            _backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            _backgroundWorker.DoWork += BackgroundWorker_DoWork;
            _backgroundWorker.RunWorkerAsync(_sessionId);
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            worker.ReportProgress(10);
            string sessionId = (string)e.Argument;
            string specialString = SpecialStringCrawler.GetSpecialString(sessionId);
            worker.ReportProgress(20);
            if (specialString == null)
            {
                e.Cancel = true;
                return;
            }

            Curriculum curriculum = CurriculumCrawler.GetCurriculum(specialString);
            StudentSaver studentSaver = new StudentSaver();
            worker.ReportProgress(50);
            StudentInfo info = DtuStudentInfoCrawler.ToStudentInfo(specialString, studentSaver);
            
            CurriculumSaver curriculumSaver = new CurriculumSaver();
            curriculumSaver.Save(curriculum);
            worker.ReportProgress(80);
            Student student = new Student(info);
            e.Result = student;
            worker.ReportProgress(100);
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                string message = "Hãy chắc chắn bạn đã đăng nhập vào MyDTU trước khi lấy Session ID, " +
                    "và đảm bảo lúc này server DTU không bảo trì. Hãy thử lại sau.";
                _messageBox.ShowMessage(message,
                                        "Thông báo",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Exclamation);
                CloseDialogWithResult(null);
            }
            else
            {
                Student student = e.Result as Student;
                string message = $"Xin chào {student.Info.Name}";
                MessageBus.Default.Publish<Cs4rsaSnackbarMessage>(new Cs4rsaSnackbarMessage(message));
                StudentResult result = new StudentResult { Student = student };
                CloseDialogWithResult(result);
            }
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressValue = e.ProgressPercentage;
        }
    }
}
