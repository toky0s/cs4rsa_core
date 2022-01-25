using CourseSearchService.Crawlers.Interfaces;
using cs4rsa_core.BaseClasses;
using cs4rsa_core.Dialogs.DialogViews;
using cs4rsa_core.Dialogs.Implements;
using cs4rsa_core.Messages;
using cs4rsa_core.Models;
using cs4rsa_core.Utils;
using cs4rsa_core.ViewModels.Interfaces;
using LightMessageBus;
using LightMessageBus.Interfaces;
using Microsoft.Toolkit.Mvvm.Input;
using SubjectCrawlService1.DataTypes;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace cs4rsa_core.ViewModels
{
    public class MainSchedulingViewModel : ViewModelBase,
        IMessageHandler<SubjectItemChangeMessage>,
        IMessageHandler<ChoicesChangedMessage>,
        IMessageHandler<UpdateSubjectDatabase>, IMainSchedulingViewModel
    {
        private string _currentYearInfo;
        private string _currentSemesterInfo;

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

        private string _shareString;
        public RelayCommand OpenSettingCommand { get; set; }
        public RelayCommand OpenUpdateWindowCommand { get; set; }
        public RelayCommand OpenAutoScheduling { get; set; }
        public RelayCommand OpenShareStringWindowCommand { get; set; }

        private readonly ShareString _shareStringGenerator;
        public MainSchedulingViewModel(ICourseCrawler courseCrawler, ShareString shareString)
        {
            _shareStringGenerator = shareString;
            MessageBus.Default.FromAny().Where<SubjectItemChangeMessage>().Notify(this);
            MessageBus.Default.FromAny().Where<ChoicesChangedMessage>().Notify(this);
            MessageBus.Default.FromAny().Where<UpdateSubjectDatabase>().Notify(this);
            CurrentSemesterInfo = courseCrawler.GetCurrentSemesterInfo();
            CurrentYearInfo = courseCrawler.GetCurrentYearInfo();

            OpenUpdateWindowCommand = new RelayCommand(OnOpenUpdateWindow, () => true);
            OpenShareStringWindowCommand = new RelayCommand(OnOpenShareStringWindow, () => true);
            TotalCredit = 0;
            TotalSubject = 0;
        }

        private void OnOpenShareStringWindow()
        {
            ShareStringUC shareStringUC = new ShareStringUC();
            ShareStringViewModel vm = (shareStringUC.DataContext as ShareStringViewModel);
            vm.ShareString = _shareString;
            vm.CloseDialogCallback = (Application.Current.MainWindow.DataContext as MainWindowViewModel).CloseDialog;
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).OpenDialog(shareStringUC);
        }

        private void OnOpenUpdateWindow()
        {
            UpdateUC updateUC = new UpdateUC();
            (updateUC.DataContext as UpdateViewModel).CloseDialogCallback = (Application.Current.MainWindow.DataContext as MainWindowViewModel).CloseDialog;
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).OpenDialog(updateUC);
        }
        public void Handle(SubjectItemChangeMessage message)
        {
            TotalCredit = message.Source.TotalCredits;
            TotalSubject = message.Source.TotalSubject;
        }

        public void Handle(ChoicesChangedMessage message)
        {
            List<ClassGroupModel> classGroupModels = message.Source;
            List<ClassGroup> classGroups = classGroupModels.Select(classGroupModels => classGroupModels.ClassGroup).ToList();
            _shareString = _shareStringGenerator.GetShareString(classGroups);
        }

        public void Handle(UpdateSubjectDatabase message)
        {
            OnOpenUpdateWindow();
        }
    }
}
