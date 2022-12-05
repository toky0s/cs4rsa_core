using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.BaseClasses;
using Cs4rsa.Constants;
using Cs4rsa.Dialogs.DialogViews;
using Cs4rsa.Messages.Publishers;
using Cs4rsa.Messages.Publishers.Dialogs;
using Cs4rsa.Services.CourseSearchSvc.Crawlers.Interfaces;
using Cs4rsa.Settings.Interfaces;
using Cs4rsa.Utils.Interfaces;

namespace Cs4rsa.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        #region Props
        private string _currentYearValue;
        public string CurrentYearValue
        {
            get { return _currentYearValue; }
            set { _currentYearValue = value; OnPropertyChanged(); }
        }

        private string _currentSemesterValue;
        public string CurrentSemesterValue
        {
            get { return _currentSemesterValue; }
            set { _currentSemesterValue = value; OnPropertyChanged(); }
        }

        private string _currentYearInfo;
        public string CurrentYearInfo
        {
            get { return _currentYearInfo; }
            set { _currentYearInfo = value; OnPropertyChanged(); }
        }

        private string _currentSemesterInfo;
        public string CurrentSemesterInfo
        {
            get { return _currentSemesterInfo; }
            set { _currentSemesterInfo = value; OnPropertyChanged(); }
        }

        private bool _isNewSemester;
        public bool IsNewSemester
        {
            get { return _isNewSemester; }
            set { _isNewSemester = value; OnPropertyChanged(); }
        }
        #endregion

        #region Commands
        public RelayCommand UpdateSubjectDatabaseCommand { get; set; }
        public RelayCommand GotoFormCommand { get; set; }
        public RelayCommand DonateCommand { get; set; }
        public RelayCommand GotoGitHubCommand { get; set; }
        public RelayCommand ManualCommand { get; set; }
        #endregion

        #region DI
        private readonly ICourseCrawler _courseCrawler;
        private readonly ISetting _setting;
        private readonly IOpenInBrowser _openInBrowser;
        #endregion

        public HomeViewModel(
            ICourseCrawler courseCrawler,
            ISetting setting,
            IOpenInBrowser openInBrowser
        )
        {
            _courseCrawler = courseCrawler;
            _setting = setting;
            _openInBrowser = openInBrowser;

            Messenger.Register<UpdateVmMsgs.UpdateSuccessMsg>(this, (r, m) =>
            {
                LoadIsNewSemester();
            });

            _currentYearValue = _courseCrawler.GetCurrentYearValue();
            _currentSemesterValue = _courseCrawler.GetCurrentSemesterValue();
            _currentSemesterInfo = _courseCrawler.GetCurrentSemesterInfo();
            _currentYearInfo = _courseCrawler.GetCurrentYearInfo();

            UpdateSubjectDatabaseCommand = new RelayCommand(OnUpdate);
            GotoFormCommand = new RelayCommand(OnGotoForm);
            GotoGitHubCommand = new RelayCommand(OnGotoGithubCommand);
            ManualCommand = new RelayCommand(OnGotoManualCommand);
            DonateCommand = new RelayCommand(OnDonate);


            LoadIsNewSemester();
        }

        private void OnDonate()
        {
            OpenDialog(new DonateUC());
        }

        private void OnGotoManualCommand()
        {
            _openInBrowser.Open(VMConstants.LK_PROJECT);
        }

        private void OnGotoGithubCommand()
        {
            _openInBrowser.Open(VMConstants.LK_PROJECT_PAGE);
        }

        private void OnGotoForm()
        {
            _openInBrowser.Open(VMConstants.LK_PROJECT_GG_SHEET);
        }

        private void OnUpdate()
        {
            Messenger.Send(new HomeVmMsgs.UpdateSubjectDbMsg(null));
        }

        public void LoadIsNewSemester()
        {
            IsNewSemester = _setting.CurrentSetting.CurrentSemesterValue != _currentSemesterValue
                || _setting.CurrentSetting.CurrentYearValue != _currentYearValue;
        }
    }
}
