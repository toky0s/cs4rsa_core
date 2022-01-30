using CourseSearchService.Crawlers.Interfaces;
using cs4rsa_core.BaseClasses;
using cs4rsa_core.Dialogs.DialogResults;
using cs4rsa_core.Dialogs.MessageBoxService;
using cs4rsa_core.Messages;
using cs4rsa_core.ModelExtensions;
using cs4rsa_core.Utils;
using cs4rsa_core.ViewModels;
using Cs4rsaDatabaseService.Interfaces;
using Cs4rsaDatabaseService.Models;
using LightMessageBus;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace cs4rsa_core.Dialogs.Implements
{
    public class ImportSessionViewModel : ViewModelBase
    {
        public Action<SessionManagerResult> CloseDialogCallback { get; set; }

        #region Properties
        public ObservableCollection<Session> ScheduleSessions { get; set; }
        public ObservableCollection<SessionDetail> ScheduleSessionDetails { get; set; }
        public ObservableCollection<SessionSchoolClass> SessionSchoolClasses { get; set; }

        private Session _selectedScheduleSession;
        public Session SelectedScheduleSession
        {
            get => _selectedScheduleSession;
            set
            {
                _selectedScheduleSession = value;
                OnPropertyChanged();
                LoadScheduleSessionDetail(value);
                CheckIsAvailableSession(value);
                ImportCommand.NotifyCanExecuteChanged();
                DeleteCommand.NotifyCanExecuteChanged();
            }
        }

        private SessionDetail _selectedSessionDetail;
        public SessionDetail SelectedSessionDetail
        {
            get { return _selectedSessionDetail; }
            set
            {
                _selectedSessionDetail = value;
                OnPropertyChanged();
                LoadSessionSchoolClasses(value);
            }
        }

        public string ShareString { get; set; }

        private int _isAvailableSession;
        public int IsAvailableSession
        {
            get => _isAvailableSession;
            set
            {
                _isAvailableSession = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Commands
        public AsyncRelayCommand DeleteCommand { get; set; }
        public RelayCommand ImportCommand { get; set; }
        public RelayCommand ShareStringCommand { get; set; }
        public RelayCommand CloseDialogCommand { get; set; }

        #endregion

        #region Services
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICourseCrawler _courseCrawler;
        private readonly SessionExtension _sessionExtension;
        private readonly IMessageBox _messageBox;
        #endregion

        public ImportSessionViewModel(IUnitOfWork unitOfWork, ICourseCrawler courseCrawler, 
            SessionExtension sessionExtension, IMessageBox messageBox)
        {
            _messageBox = messageBox;
            _sessionExtension = sessionExtension;
            _unitOfWork = unitOfWork;
            _courseCrawler = courseCrawler;

            ScheduleSessions = new();
            ScheduleSessionDetails = new();
            SessionSchoolClasses = new();
            _isAvailableSession = -1;

            DeleteCommand = new AsyncRelayCommand(OnDelete, CanDelete);
            ImportCommand = new RelayCommand(OnImport, CanImport);
            ShareStringCommand = new RelayCommand(OnParseShareString, CanParse);
            CloseDialogCommand = new RelayCommand(OnCloseDialog);
        }

        private void LoadScheduleSessionDetail(Session value)
        {
            if (value != null)
            {
                ScheduleSessionDetails.Clear();
                SessionSchoolClasses.Clear();
                IEnumerable<SessionDetail> details = _unitOfWork.Sessions.GetSessionDetails(value.SessionId);
                foreach (SessionDetail item in details)
                {
                    ScheduleSessionDetails.Add(item);
                }
            }
        }

        private void LoadSessionSchoolClasses(SessionDetail value)
        {
            if(value != null)
            {
                SessionSchoolClasses.Clear();
                IEnumerable<SessionSchoolClass> sessionSchoolClasses = _unitOfWork
                    .SessionSchoolClassRepository
                    .GetSessionSchoolClass(value);
                foreach (SessionSchoolClass sessionSchoolClass in sessionSchoolClasses)
                {
                    SessionSchoolClasses.Add(sessionSchoolClass);
                }
            }
        }

        private void CheckIsAvailableSession(Session session)
        {
            if (session != null)
            {
                string semester = session.SemesterValue;
                string year = session.YearValue;
                IsAvailableSession = semester.Equals(_courseCrawler.GetCurrentSemesterValue(), StringComparison.Ordinal)
                    && year.Equals(_courseCrawler.GetCurrentYearValue(), StringComparison.Ordinal) ? 1 : 0;
            }
            else
            {
                IsAvailableSession = -1;
            }

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
            ShareString shareString = new(_unitOfWork, _courseCrawler);
            SessionManagerResult result = shareString.GetSubjectFromShareString(ShareString);
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).CloseDialog();
            MessageBus.Default.Publish(new ExitImportSubjectMessage(result));
        }

        private bool CanImport()
        {
            if (_selectedScheduleSession != null && _sessionExtension.IsValid(_selectedScheduleSession))
            {
                return true;
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
            sessions.ToList().ForEach(session => ScheduleSessions.Add(session));
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
                    SubjectName = item.SubjectName,
                    RegisterCode = item.RegisterCode
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
            MessageBoxResult result = _messageBox.ShowMessage($"Bạn có chắc muốn xoá phiên {sessionName}?",
                                    "Thông báo",
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                _unitOfWork.Sessions.Remove(_selectedScheduleSession);
                _unitOfWork.Complete();
                await Reload();
            }
        }

        private async Task Reload()
        {
            ScheduleSessions.Clear();
            ScheduleSessionDetails.Clear();
            SessionSchoolClasses.Clear();
            await LoadScheduleSession();
        }
    }
}
