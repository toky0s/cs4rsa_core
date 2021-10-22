using cs4rsa_core.Dialogs.DialogResults;
using cs4rsa_core.Dialogs.MessageBoxService;
using cs4rsa_core.Models;
using cs4rsa_core.BaseClasses;
using cs4rsa_core.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.Toolkit.Mvvm.Input;
using Cs4rsaDatabaseService.DataProviders;
using CourseSearchService.Crawlers.Interfaces;
using System.Linq;
using Cs4rsaDatabaseService.Models;
using Cs4rsaDatabaseService.Interfaces;
using cs4rsa_core.ModelExtensions;
using cs4rsa_core.ViewModels;
using cs4rsa_core.Messages;
using LightMessageBus;

namespace cs4rsa_core.Dialogs.Implements
{
    class ImportSessionViewModel: ViewModelBase
    {
        public ObservableCollection<Session> ScheduleSessions { get; set; } = new();
        public ObservableCollection<SessionDetail> ScheduleSessionDetails { get; set; } = new();

        private Session _selectedScheduleSession;
        public Session SelectedScheduleSession
        {
            get => _selectedScheduleSession;
            set
            {
                _selectedScheduleSession = value;
                OnPropertyChanged();
                ImportCommand.NotifyCanExecuteChanged();
                DeleteCommand.NotifyCanExecuteChanged();
                LoadScheduleSessionDetail(value);
            }
        }

        public string ShareString { get; set; }

        private void LoadScheduleSessionDetail(Session value)
        {
            if (value != null)
            {
                ScheduleSessionDetails.Clear();
                List<SessionDetail> details = _unitOfWork.Sessions.GetSessionDetails(value.SessionId);
                foreach (SessionDetail item in details)
                {
                    ScheduleSessionDetails.Add(item);
                }
            }
        }

        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand ImportCommand { get; set; }
        public RelayCommand ShareStringCommand { get; set; }
        public RelayCommand CloseDialogCommand { get; set; }
        public IMessageBox MessageBox { get; set; }

        public Action<SessionManagerResult> CloseDialogCallback { get; set; }

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICourseCrawler _courseCrawler;
        private readonly SessionExtension _sessionExtension;
        public ImportSessionViewModel(IUnitOfWork unitOfWork, ICourseCrawler courseCrawler, SessionExtension sessionExtension)
        {
            _sessionExtension = sessionExtension;
            _unitOfWork = unitOfWork;
            _courseCrawler = courseCrawler;
            DeleteCommand = new RelayCommand(OnDelete, CanDelete);
            ImportCommand = new RelayCommand(OnImport, CanImport);
            ShareStringCommand = new RelayCommand(OnParseShareString, CanParse);
            CloseDialogCommand = new RelayCommand(OnCloseDialog);
        }

        private void OnCloseDialog()
        {
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).CloseDialog();
        }

        private bool CanParse()
        {
            return true;
        }

        private void OnParseShareString()
        {
            ShareString shareString = new ShareString(_unitOfWork, _courseCrawler);
            SessionManagerResult result = shareString.GetSubjectFromShareString(ShareString);
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).CloseDialog();
            MessageBus.Default.Publish(new ExitImportSubjectMessage(result));
        }

        private bool CanImport()
        {
            if (_selectedScheduleSession != null)
            {
                if (_sessionExtension.IsValid(_selectedScheduleSession))
                {
                    return true;
                }
            }
            return false;
        }

        private bool CanDelete()
        {
            if (_selectedScheduleSession != null)
                return true;
            return false;
        }

        public void LoadScheduleSession()
        {
            ScheduleSessions.Clear();
            List<Session> sessions = _unitOfWork.Sessions.GetAll().ToList();
            foreach (Session item in sessions)
            {
                ScheduleSessions.Add(item);
            }
        }

        private void OnImport()
        {
            List<SubjectInfoData> subjectInfoDatas = new List<SubjectInfoData>();
            foreach (SessionDetail item in ScheduleSessionDetails)
            {
                SubjectInfoData data = new SubjectInfoData()
                {
                    SubjectCode = item.SubjectCode,
                    ClassGroup = item.ClassGroup,
                    SubjectName = item.SubjectName
                };
                subjectInfoDatas.Add(data);
            }
            SessionManagerResult result = new SessionManagerResult(subjectInfoDatas);
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).CloseDialog();
            MessageBus.Default.Publish(new ExitImportSubjectMessage(result));
        }

        private void OnDelete()
        {
            string sessionName = _selectedScheduleSession.Name;
            MessageBoxResult result = MessageBox.ShowMessage($"Bạn có chắc muốn xoá phiên {sessionName}?",
                                    "Thông báo",
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                _unitOfWork.Sessions.Remove(_selectedScheduleSession);
                _unitOfWork.Complete();
                Reload();
            }
        }

        private void Reload()
        {
            ScheduleSessions.Clear();
            ScheduleSessionDetails.Clear();
            LoadScheduleSession();
        }
    }
}
