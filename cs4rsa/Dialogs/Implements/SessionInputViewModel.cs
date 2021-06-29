using cs4rsa.BasicData;
using cs4rsa.Crawler;
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

            string curid = CurIdCrawler.GetCurId(specialString);
            worker.ReportProgress(30);
            StudentSaver studentSaver = new StudentSaver();
            StudentInfo info = DtuStudentInfoCrawler.ToStudentInfo(specialString, studentSaver);
            worker.ReportProgress(40);
            string t = Helpers.Helpers.GetTimeFromEpoch();
            string url2 = $"https://mydtu.duytan.edu.vn/Modules/curriculuminportal/ajax/LoadChuongTrinhHocEachPart.aspx?t={t}&studentidnumber={specialString}&acaLevid=3&curid={curid}&cursectionid=2002";
            string url1 = $"https://mydtu.duytan.edu.vn/Modules/curriculuminportal/ajax/LoadChuongTrinhHocEachPart.aspx?t={t}&studentidnumber={specialString}&acaLevid=3&curid={curid}&cursectionid=2001";
            string url3 = $"https://mydtu.duytan.edu.vn/Modules/curriculuminportal/ajax/LoadChuongTrinhHocEachPart.aspx?t={t}&studentidnumber={specialString}&acaLevid=3&curid={curid}&cursectionid=2003";
            string url4 = $"https://mydtu.duytan.edu.vn/Modules/curriculuminportal/ajax/LoadChuongTrinhHocEachPart.aspx?t={t}&studentidnumber={specialString}&acaLevid=3&curid={curid}&cursectionid=2004";

            PreParGetter preParGetter = new PreParGetter();
            ProgramSubjectSaver programSubjectSaver = new ProgramSubjectSaver();
            StudentProgramCrawler programCrawler1 = new StudentProgramCrawler(sessionId, url1, programSubjectSaver, preParGetter);
            worker.ReportProgress(50);
            StudentProgramCrawler programCrawler2 = new StudentProgramCrawler(sessionId, url2, programSubjectSaver, preParGetter);
            worker.ReportProgress(60);
            StudentProgramCrawler programCrawler3 = new StudentProgramCrawler(sessionId, url3, programSubjectSaver, preParGetter);
            worker.ReportProgress(70);
            StudentProgramCrawler programCrawler4 = new StudentProgramCrawler(sessionId, url4, programSubjectSaver, preParGetter);
            worker.ReportProgress(100);
            ProgramDiagram diagram = new ProgramDiagram(programCrawler1.Root, programCrawler2.Root, 
                                                        programCrawler3.Root, programCrawler4.Root);
            Student student = new Student(info, diagram);
            e.Result = student;
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
