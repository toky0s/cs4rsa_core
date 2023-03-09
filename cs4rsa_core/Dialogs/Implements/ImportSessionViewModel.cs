using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.BaseClasses;
using Cs4rsa.Constants;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;
using Cs4rsa.Dialogs.DialogResults;
using Cs4rsa.Messages.Publishers.Dialogs;
using Cs4rsa.Services.CourseSearchSvc.Crawlers;
using Cs4rsa.Utils;

using MaterialDesignThemes.Wpf;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Cs4rsa.Dialogs.Implements
{
    public partial class ImportSessionViewModel : ViewModelBase
    {
        #region Properties
        public ObservableCollection<UserSchedule> ScheduleSessions { get; set; }
        public ObservableCollection<UserSubject> UserSubjects { get; set; }

        [ObservableProperty]
        private UserSchedule _selectedScheduleSession;

        [ObservableProperty]
        private ScheduleDetail _selectedSessionDetail;

        [ObservableProperty]
        private int _isAvailableSession;

        [ObservableProperty]
        private bool _isUseCache;

        public string ShareStringText { get; set; }

        #endregion

        #region Commands
        public AsyncRelayCommand DeleteCommand { get; set; }
        public RelayCommand ImportCommand { get; set; }
        public RelayCommand CloseDialogCommand { get; set; }
        #endregion

        #region Services
        private readonly IUnitOfWork _unitOfWork;
        private readonly CourseCrawler _courseCrawler;
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;
        #endregion

        public ImportSessionViewModel(
            IUnitOfWork unitOfWork,
            CourseCrawler courseCrawler,
            ISnackbarMessageQueue snackbarMessageQueue)
        {
            _unitOfWork = unitOfWork;
            _courseCrawler = courseCrawler;
            _snackbarMessageQueue = snackbarMessageQueue;

            ScheduleSessions = new();
            UserSubjects = new();
            _isAvailableSession = -1;

            DeleteCommand = new AsyncRelayCommand(OnDelete, () => _selectedScheduleSession != null);
            ImportCommand = new RelayCommand(OnImport, () => UserSubjects.Any());
            CloseDialogCommand = new RelayCommand(CloseDialog);
        }

        partial void OnSelectedScheduleSessionChanged(UserSchedule value)
        {
            LoadUserSubject(value);
            CheckIsAvailableSession(value);
            ImportCommand.NotifyCanExecuteChanged();
            DeleteCommand.NotifyCanExecuteChanged();
        }

        public void OnCopyRegisterCode(string registerCode)
        {
            Clipboard.SetText(registerCode);
            _snackbarMessageQueue.Enqueue(VmConstants.SnbCopySuccess + VmConstants.StrSpace + registerCode);
        }

        private void LoadUserSubject(UserSchedule userSchedule)
        {
            if (userSchedule != null)
            {
                UserSubjects.Clear();
                IEnumerable<UserSubject> userSubjects = _unitOfWork.UserSchedules
                    .GetSessionDetails(userSchedule.UserScheduleId)
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

        public void LoadShareString(string shareString)
        {
            UserSubjects.Clear();
            if (!string.IsNullOrEmpty(shareString))
            {
                IEnumerable<UserSubject> userSubjects = ShareString.GetSubjectFromShareString(shareString);
                if (userSubjects != null)
                {
                    foreach (UserSubject userSubject in userSubjects)
                        UserSubjects.Add(userSubject);
                }
            }
        }

        private void CheckIsAvailableSession(UserSchedule session)
        {
            if (session != null)
            {
                string semester = session.SemesterValue;
                string year = session.YearValue;
                int available = 1;
                int unavailable = 0;
                IsAvailableSession = semester.Equals(_courseCrawler.CurrentSemesterValue, StringComparison.Ordinal)
                    && year.Equals(_courseCrawler.CurrentYearValue, StringComparison.Ordinal) ? available : unavailable;
            }
            else
            {
                IsAvailableSession = -1;
            }
        }

        public async Task LoadScheduleSession()
        {
            ScheduleSessions.Clear();
            UserSubjects.Clear();
            IAsyncEnumerable<UserSchedule> sessions = _unitOfWork.UserSchedules.GetAll();
            await foreach (UserSchedule session in sessions)
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
            MessageBoxResult result = MessageBox.Show($"Bạn có chắc muốn xoá phiên {sessionName}?",
                                                        "Thông báo",
                                                        MessageBoxButton.YesNo,
                                                        MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                await _unitOfWork.BeginTransAsync();
                _unitOfWork.UserSchedules.Remove(_selectedScheduleSession);
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
