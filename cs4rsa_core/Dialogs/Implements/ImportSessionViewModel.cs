using cs4rsa_core.Dialogs.DialogResults;
using cs4rsa_core.Dialogs.MessageBoxService;
using cs4rsa_core.BaseClasses;
using cs4rsa_core.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.Toolkit.Mvvm.Input;
using CourseSearchService.Crawlers.Interfaces;
using Cs4rsaDatabaseService.Models;
using Cs4rsaDatabaseService.Interfaces;
using cs4rsa_core.ModelExtensions;
using cs4rsa_core.ViewModels;
using cs4rsa_core.Messages;
using LightMessageBus;
using System.Threading.Tasks;

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

        public AsyncRelayCommand DeleteCommand { get; set; }
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
            DeleteCommand = new AsyncRelayCommand(OnDelete, CanDelete);
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
            return _selectedScheduleSession != null;
        }

        public async Task LoadScheduleSession()
        {
            ScheduleSessions.Clear();
            IEnumerable<Session> sessions = await _unitOfWork.Sessions.GetAllAsync();
            foreach (Session item in sessions)
            {
                ScheduleSessions.Add(item);
            }
        }

        private void OnImport()
        {
            List<SubjectInfoData> subjectInfoDatas = new();
            foreach (SessionDetail item in ScheduleSessionDetails)
            {
                SubjectInfoData data = new()
                {
                    SubjectCode = item.SubjectCode,
                    ClassGroup = item.ClassGroup,
                    SubjectName = item.SubjectName
                };
                subjectInfoDatas.Add(data);
            }
            SessionManagerResult result = new(subjectInfoDatas);
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).CloseDialog();
            MessageBus.Default.Publish(new ExitImportSubjectMessage(result));
        }

        private async Task OnDelete()
        {
            string sessionName = _selectedScheduleSession.Name;
            MessageBoxResult result = MessageBox.ShowMessage($"Bạn có chắc muốn xoá phiên {sessionName}?",
                                    "Thông báo",
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                _unitOfWork.Sessions.Remove(_selectedScheduleSession);
                await _unitOfWork.CompleteAsync();
                await Reload();
            }
        }

        private async Task Reload()
        {
            ScheduleSessions.Clear();
            ScheduleSessionDetails.Clear();
            await LoadScheduleSession();
        }
    }
}
