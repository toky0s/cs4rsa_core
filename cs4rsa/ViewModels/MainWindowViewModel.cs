using cs4rsa.Crawler;
using cs4rsa.BaseClasses;
using LightMessageBus;
using LightMessageBus.Interfaces;
using cs4rsa.Messages;
using System.Windows;
using System;
using cs4rsa.Dialogs.MessageBoxService;
using cs4rsa.Dialogs.DialogService;
using cs4rsa.Dialogs.DialogViews;
using cs4rsa.Dialogs.Implements;
using cs4rsa.Dialogs.DialogResults;

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
        public RelayCommand OpenAutoScheduling { get; set; }

        public MainWindowViewModel(IMessageBox messageBox)
        {
            MessageBus.Default.FromAny().Where<SubjectItemChangeMessage>().Notify(this);
            _messageBox = messageBox;
            HomeCourseSearch homeCourseSearch = HomeCourseSearch.GetInstance();
            CurrentSemesterInfo = homeCourseSearch.CurrentSemesterInfo;
            CurrentYearInfo = homeCourseSearch.CurrentYearInfo;
            OpenUpdateWindowCommand = new RelayCommand(OnOpenUpdateWindow, () => true);
            OpenSettingCommand = new RelayCommand(OnOpenSetting, () => true);
            OpenAutoScheduling = new RelayCommand(OnOpenAutoScheduling, () => true);
            TotalCredit = 0;
            TotalSubject = 0;
        }


        /// <summary>
        /// Mở chức năng tự động xếp lịch
        /// </summary>
        /// <param name="obj"></param>
        private void OnOpenAutoScheduling(object obj)
        {
            throw new NotImplementedException();
        }

        private void OnOpenUpdateWindow(object obj)
        {
            Cs4rsaMessageBox cs4RsaMessageBox = new Cs4rsaMessageBox();
            UpdateWindow updateWindow = new UpdateWindow();
            UpdateViewModel updateViewModel = new UpdateViewModel(cs4RsaMessageBox);
            UpdateResult result = DialogService<UpdateResult>.OpenDialog(updateViewModel, updateWindow, obj as Window);
            MessageBus.Default.Publish(new UpdateSuccessMessage(null));
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
