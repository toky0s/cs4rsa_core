using cs4rsa_core.BaseClasses;
using cs4rsa_core.Dialogs.DialogResults;
using cs4rsa_core.Dialogs.MessageBoxService;
using cs4rsa_core.Messages.Publishers.Dialogs;
using cs4rsa_core.Utils;
using MaterialDesignThemes.Wpf;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using cs4rsa_core.Cs4rsaDatabase.Interfaces;
using cs4rsa_core.Cs4rsaDatabase.Models;
using cs4rsa_core.Services.CourseSearchSvc.Crawlers.Interfaces;
using System.Linq;
using cs4rsa_core.Constants;

namespace cs4rsa_core.Dialogs.Implements
{
    public class ImportSessionViewModel : ViewModelBase
    {
        #region Properties
        public ObservableCollection<Session> ScheduleSessions { get; set; }
        public ObservableCollection<UserSubject> UserSubjects { get; set; }

        private Session _selectedScheduleSession;
        public Session SelectedScheduleSession
        {
            get => _selectedScheduleSession;
            set
            {
                _selectedScheduleSession = value;
                OnPropertyChanged();
                LoadUserSubject(value);
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
            }
        }

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

        public string ShareString { get; set; }

        #endregion

        #region Commands
        public AsyncRelayCommand DeleteCommand { get; set; }
        public RelayCommand ImportCommand { get; set; }
        public RelayCommand CloseDialogCommand { get; set; }
        #endregion

        #region Services
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICourseCrawler _courseCrawler;
        private readonly IMessageBox _messageBox;
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;
        private readonly ShareString _shareStringHelper;
        #endregion

        public ImportSessionViewModel(
            IUnitOfWork unitOfWork, 
            ICourseCrawler courseCrawler,
            IMessageBox messageBox, 
            ISnackbarMessageQueue snackbarMessageQueue,
            ShareString shareString)
        {
            _messageBox = messageBox;
            _unitOfWork = unitOfWork;
            _courseCrawler = courseCrawler;
            _snackbarMessageQueue = snackbarMessageQueue;
            _shareStringHelper = shareString;

            ScheduleSessions = new();
            UserSubjects = new();
            _isAvailableSession = -1;

            DeleteCommand = new AsyncRelayCommand(OnDelete, CanDelete);
            ImportCommand = new RelayCommand(OnImport, CanImport);
            CloseDialogCommand = new RelayCommand(CloseDialog);
        }

        public void OnCopyRegisterCode(string registerCode)
        {
            Clipboard.SetText(registerCode);
            _snackbarMessageQueue.Enqueue(VMConstants.SNB_COPY_SUCCESS + " " + registerCode);
        }

        private void LoadUserSubject(Session session)
        {
            if (session != null)
            {
                UserSubjects.Clear();
                IEnumerable<UserSubject> userSubjects = _unitOfWork.Sessions
                    .GetSessionDetails(session.SessionId)
                    .Select(sd => new UserSubject()
                            {
                                SubjectCode = sd.SubjectCode,
                                SubjectName = sd.SubjectName,
                                ClassGroup = sd.ClassGroup,
                                SchoolClass = sd.SelectedSchoolClass,
                                RegisterCode = sd.RegisterCode
                            }
                    );

                foreach (UserSubject userSubject in userSubjects) 
                    UserSubjects.Add(userSubject);
            }
        }

        internal void LoadShareString(string shareString)
        {
            UserSubjects.Clear();
            if (!string.IsNullOrEmpty(shareString))
            {
                IEnumerable<UserSubject> userSubjects = _shareStringHelper.GetSubjectFromShareString(shareString);
                if (userSubjects != null)
                {
                    foreach (UserSubject userSubject in userSubjects)
                        UserSubjects.Add(userSubject);
                }
            }
        }

        private void CheckIsAvailableSession(Session session)
        {
            if (session != null)
            {
                string semester = session.SemesterValue;
                string year = session.YearValue;
                int available = 1;
                int unavailable = 0;
                IsAvailableSession = semester.Equals(_courseCrawler.GetCurrentSemesterValue(), StringComparison.Ordinal)
                    && year.Equals(_courseCrawler.GetCurrentYearValue(), StringComparison.Ordinal) ? available : unavailable;
            }
            else
            {
                IsAvailableSession = -1;
            }
        }

        private bool CanImport()
        {
            return UserSubjects.Any();
        }

        private bool CanDelete()
        {
            return _selectedScheduleSession != null;
        }

        public async Task LoadScheduleSession()
        {
            ScheduleSessions.Clear();
            UserSubjects.Clear();
            IEnumerable<Session> sessions = await _unitOfWork.Sessions.GetAllAsync();
            foreach (Session session in sessions)
            {
                ScheduleSessions.Add(session);
            }
        }

        private void OnImport()
        {
            CloseDialog();
            Messenger.Send(new ImportSessionVmMsgs.ExitImportSubjectMsg(UserSubjects));
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
                await _unitOfWork.BeginTransAsync();
                _unitOfWork.Sessions.Remove(_selectedScheduleSession);
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitAsync();
                await Reload();
            }
        }

        private async Task Reload()
        {
            ScheduleSessions.Clear();
            UserSubjects.Clear();
            await LoadScheduleSession();
        }
    }
}
