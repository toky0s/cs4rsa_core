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
        public RelayCommand FindCommand { get; set; }
        public SessionInputViewModel(IMessageBox messageBox)
        {
            _messageBox = messageBox;
            FindCommand = new RelayCommand(OnFind, () => true);
        }

        private void OnFind(object obj)
        {
            BackgroundWorker backgroundWorker = new BackgroundWorker()
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.RunWorkerAsync(_sessionId);
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            worker.ReportProgress(10);
            string sessionId = (string)e.Argument;
            string specialString = SpecialStringCrawler.GetSpecialString(sessionId);
            if (specialString == null)
            {
                e.Cancel = true;
                return;
            }

            Curriculum curriculum = CurriculumCrawler.GetCurriculum(specialString);
            StudentSaver studentSaver = new StudentSaver();
            StudentInfo info = DtuStudentInfoCrawler.ToStudentInfo(specialString, studentSaver);

            if (!Cs4rsaDataView.IsExistsCurriculum(info.Curriculum.CurId))
            {
                ProgramDiagramCrawler programDiagramCrawler = new ProgramDiagramCrawler(_sessionId, specialString, worker);
                ProgramDiagram diagram = programDiagramCrawler.ToProgramDiagram();
            }
            
            CurriculumSaver curriculumSaver = new CurriculumSaver();
            curriculumSaver.Save(curriculum);

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
                SessionId = "";
                ProgressValue = 0;
            }
            else
            {
                Student student = e.Result as Student;
                string message = $"Xin chào {student.Info.Name}";
                _messageBox.ShowMessage(message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
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
