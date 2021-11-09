﻿using LightMessageBus.Interfaces;
using LightMessageBus;
using cs4rsa_core.BaseClasses;
using cs4rsa_core.Messages;
using Microsoft.Toolkit.Mvvm.Input;
using CourseSearchService.Crawlers.Interfaces;
using cs4rsa_core.Settings.Interfaces;
using System;
using cs4rsa_core.Interfaces;

namespace cs4rsa_core.ViewModels
{
    public class HomeViewModel : ViewModelBase, IMessageHandler<UpdateSuccessMessage>
    {
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

        public RelayCommand UpdateSubjectDatabaseCommand { get; set; }
        public RelayCommand GotoFacebookCommand { get; set; }
        public RelayCommand GotoGitHubCommand { get; set; }

        private readonly ICourseCrawler _courseCrawler;
        private readonly ISetting _setting;
        private readonly IOpenInBrowser _openInBrowser;
        public HomeViewModel(ICourseCrawler courseCrawler, ISetting setting, IOpenInBrowser openInBrowser)
        {
            _courseCrawler = courseCrawler;
            _setting = setting;
            _openInBrowser = openInBrowser;

            MessageBus.Default.FromAny().Where<UpdateSuccessMessage>().Notify(this);
            _currentYearValue = _courseCrawler.GetCurrentYearValue();
            _currentSemesterValue = _courseCrawler.GetCurrentSemesterValue();
            _currentSemesterInfo = _courseCrawler.GetCurrentSemesterInfo();
            _currentYearInfo = _courseCrawler.GetCurrentYearInfo();
            
            UpdateSubjectDatabaseCommand = new RelayCommand(OnUpdate);
            GotoFacebookCommand = new RelayCommand(OnGotoFaceBook);
            GotoGitHubCommand = new RelayCommand(OnGotoGithubCommand);

            LoadIsNewSemester();
        }

        private void OnGotoGithubCommand()
        {
            _openInBrowser.Open("https://github.com/toky0s/cs4rsa_core");
        }

        private void OnGotoFaceBook()
        {
            _openInBrowser.Open("https://www.facebook.com/truongaxin/");
        }

        private void OnUpdate()
        {
            MessageBus.Default.Publish(new UpdateSubjectDatabase(null));
        }

        public void LoadIsNewSemester()
        {
            IsNewSemester = _setting.CurrentSetting.CurrentSemesterValue != _currentSemesterValue || _setting.CurrentSetting.CurrentYearValue != _currentYearValue;
        }

        public void Handle(UpdateSuccessMessage message)
        {
            LoadIsNewSemester();
        }
    }
}