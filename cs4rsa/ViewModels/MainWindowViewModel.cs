using cs4rsa.Crawler;
using cs4rsa.BaseClasses;
using LightMessageBus;
using LightMessageBus.Interfaces;
using cs4rsa.Messages;
using System.Windows;
using System;
using cs4rsa.Dialogs.MessageBoxService;

namespace cs4rsa.ViewModels
{
    public class MainWindowViewModel: NotifyPropertyChangedBase, IMessageHandler<SubjectItemChangeMessage>
    {
        private string _currentYearInfo;
        private string _currentSemesterInfo;

        public string CurrentYearInfo
        {
            get
            {
                return _currentYearInfo;
            }
            set
            {
                _currentYearInfo = value;
                RaisePropertyChanged();
            }
        }
        public string CurrentSemesterInfo
        {
            get
            {
                return _currentSemesterInfo;
            }
            set
            {
                _currentSemesterInfo = value;
                RaisePropertyChanged();
            }
        }

        private int _totalCredit = 0;
        public int TotalCredit
        {
            get
            {
                return _totalCredit;
            }
            set
            {
                _totalCredit = value;
                RaisePropertyChanged();
            }
        }

        private int _totalSubject = 0;
        public int TotalSubject
        {
            get
            {
                return _totalSubject;
            }
            set
            {
                _totalSubject = value;
                RaisePropertyChanged();
            }
        }

        private IMessageBox _messageBox;

        public RelayCommand OpenSettingCommand { get; set; }
        public RelayCommand OpenUpdateWindowCommand { get; set; }

        public MainWindowViewModel(IMessageBox messageBox)
        {
            MessageBus.Default.FromAny().Where<SubjectItemChangeMessage>().Notify(this);
            _messageBox = messageBox;
            HomeCourseSearch homeCourseSearch = HomeCourseSearch.GetInstance();
            CurrentSemesterInfo = homeCourseSearch.CurrentSemesterInfo;
            CurrentYearInfo = homeCourseSearch.CurrentYearInfo;
            OpenUpdateWindowCommand = new RelayCommand(OnOpenUpdateWindow, () => true);
            OpenSettingCommand = new RelayCommand(OnOpenSetting, () => true);
            TotalCredit = 0;
            TotalSubject = 0;
        }

        private void OnOpenUpdateWindow(object obj)
        {
            string message = "Thao tác cập nhật cơ sở dữ liệu sẽ cập nhật lại toàn bộ dữ liệu môn học. Sẽ mất một chút thời gian!";
            string caption = "Cập nhật";
            MessageBoxResult result = _messageBox.ShowMessage(message, caption, MessageBoxButton.OKCancel, MessageBoxImage.Asterisk);
            if (result == MessageBoxResult.OK)
            {
                _messageBox.ShowMessage("Start Update");
            }
        }

        public void Handle(SubjectItemChangeMessage message)
        {
            TotalCredit = message.Source.TotalCredits;
            TotalSubject = message.Source.TotalSubject;
        }

        private void OnOpenSetting(object obj)
        {
            MessageBox.Show("Mở setting");
        }
    }
}
