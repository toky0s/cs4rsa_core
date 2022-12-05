﻿using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.BaseClasses;
using Cs4rsa.Messages.Publishers;
using Cs4rsa.Services.CourseSearchSvc.Crawlers.Interfaces;
using Cs4rsa.ViewModels.Interfaces;

using MaterialDesignThemes.Wpf;

namespace Cs4rsa.ViewModels
{
    public class MainSchedulingViewModel : ViewModelBase,
        IMainSchedulingViewModel
    {
        #region Fields
        private string _currentYearInfo;
        private string _currentSemesterInfo;
        #endregion

        #region Bindings
        public string CurrentYearInfo
        {
            get => _currentYearInfo;
            set
            {
                _currentYearInfo = value;
                OnPropertyChanged();
            }
        }
        public string CurrentSemesterInfo
        {
            get => _currentSemesterInfo;
            set
            {
                _currentSemesterInfo = value;
                OnPropertyChanged();
            }
        }

        private int _totalCredit;
        public int TotalCredit
        {
            get => _totalCredit;
            set
            {
                _totalCredit = value;
                OnPropertyChanged();
            }
        }

        private int _totalSubject;
        public int TotalSubject
        {
            get => _totalSubject;
            set
            {
                _totalSubject = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Commands
        public RelayCommand OpenSettingCommand { get; set; }
        public RelayCommand OpenAutoScheduling { get; set; }
        #endregion

        public MainSchedulingViewModel(ICourseCrawler courseCrawler, ISnackbarMessageQueue snackbarMessageQueue)
        {

            Messenger.Register<SearchVmMsgs.SubjectItemChangedMsg>(this, (r, m) =>
            {
                TotalCredit = m.Value.Item1;
                TotalSubject = m.Value.Item2;
            });

            CurrentSemesterInfo = courseCrawler.GetCurrentSemesterInfo();
            CurrentYearInfo = courseCrawler.GetCurrentYearInfo();

            TotalCredit = 0;
            TotalSubject = 0;
        }
    }
}
