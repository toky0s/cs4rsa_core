﻿using CourseSearchService.Crawlers.Interfaces;
using cs4rsa_core.BaseClasses;
using cs4rsa_core.Dialogs.DialogViews;
using cs4rsa_core.Interfaces;
using cs4rsa_core.Messages.Publishers;
using cs4rsa_core.Messages.Publishers.Dialogs;
using cs4rsa_core.Settings.Interfaces;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace cs4rsa_core.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        #region Fields
        private static DonateUC _donateUC;
        #endregion

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

            WeakReferenceMessenger.Default.Register<UpdateVmMsgs.UpdateSuccessMsg>(this, (r, m) =>
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

            _donateUC = new();

            LoadIsNewSemester();
        }

        private void OnDonate()
        {
            OpenDialog(_donateUC);
        }

        private void OnGotoManualCommand()
        {
            _openInBrowser.Open("https://toky0s.github.io/cs4rsa_core/");
        }

        private void OnGotoGithubCommand()
        {
            _openInBrowser.Open("https://github.com/toky0s/cs4rsa_core");
        }

        private void OnGotoForm()
        {
            _openInBrowser.Open("https://forms.gle/JHipUM7knjbqKGKWA");
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
