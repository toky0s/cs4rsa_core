using Cs4rsa.Database.Interfaces;
using Cs4rsa.Database.Models;
using Cs4rsa.Module.ManuallySchedule.Dialogs.Models;
using Cs4rsa.Module.ManuallySchedule.Events;
using Cs4rsa.Module.ManuallySchedule.Utils;
using Cs4rsa.Service.CourseCrawler.Interfaces;
using Cs4rsa.Service.Dialog.Events;

using MaterialDesignThemes.Wpf;

using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Cs4rsa.Module.ManuallySchedule.Dialogs.ViewModels
{
    public class ImportSessionUCViewModel : BindableBase
    {
        #region Properties
        public ObservableCollection<UserSchedule> ScheduleSessions { get; set; }
        public ObservableCollection<UserSubject> UserSubjects { get; set; }

        private UserSchedule _selectedScheduleSession;
        public UserSchedule SelectedScheduleSession
        {
            get { return _selectedScheduleSession; }
            set 
            { 
                SetProperty(ref _selectedScheduleSession, value);
                LoadUserSubject(value);
                CheckIsAvailableSession(value);
                ImportCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
            }
        }

        private ScheduleDetail _selectedSessionDetail;
        public ScheduleDetail SelectedSessionDetail
        {
            get { return _selectedSessionDetail; }
            set { SetProperty(ref _selectedSessionDetail, value); }
        }

        private int _isAvailableSession;
        public int IsAvailableSession
        {
            get { return _isAvailableSession; }
            set { SetProperty(ref _isAvailableSession, value); }
        }

        private bool _isUseCache;
        public bool IsUseCache
        {
            get { return _isUseCache; }
            set { SetProperty(ref _isUseCache, value); }
        }

        private string _shareStringText;
        public string ShareStringText
        {
            get { return _shareStringText; }
            set { SetProperty(ref _shareStringText, value); }
        }

        #endregion

        #region Commands
        public DelegateCommand DeleteCommand { get; set; }
        public DelegateCommand ImportCommand { get; set; }
        public DelegateCommand CloseDialogCommand { get; set; }
        #endregion

        #region Services
        private readonly IEventAggregator _eventAggregator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICourseCrawler _courseCrawler;
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;
        #endregion

        public ImportSessionUCViewModel(
            IEventAggregator eventAggregator,
            IUnitOfWork unitOfWork, 
            ICourseCrawler courseCrawler, 
            ISnackbarMessageQueue snackbarMessageQueue)
        {
            _eventAggregator = eventAggregator;
            _unitOfWork = unitOfWork;
            _courseCrawler = courseCrawler;
            _snackbarMessageQueue = snackbarMessageQueue;

            ScheduleSessions = new ObservableCollection<UserSchedule>();
            UserSubjects = new ObservableCollection<UserSubject>();
            _isAvailableSession = -1;

            DeleteCommand = new DelegateCommand(OnDelete, () => _selectedScheduleSession != null);
            ImportCommand = new DelegateCommand(
                OnImport,
                () => UserSubjects.Any() && IsAvailableSession == 1
                    || UserSubjects.Count > 0 && !string.IsNullOrWhiteSpace(ShareStringText)
            );
            CloseDialogCommand = new DelegateCommand(() => _eventAggregator.GetEvent<CloseDialogEvent>().Publish());
        }

        public void OnCopyRegisterCode(string registerCode)
        {
            Clipboard.SetText(registerCode);
            _snackbarMessageQueue.Enqueue($"Đã sao chép {registerCode}");
        }

        private void LoadUserSubject(UserSchedule userSchedule)
        {
            if (userSchedule != null)
            {
                UserSubjects.Clear();
                var userSubjects = _unitOfWork.UserSchedules
                    .GetSessionDetails(userSchedule.UserScheduleId)
                    .Select(
                        sd => new UserSubject()
                        {
                            SubjectCode = sd.SubjectCode,
                            SubjectName = sd.SubjectName,
                            ClassGroup = sd.ClassGroup,
                            SchoolClass = sd.SelectedSchoolClass,
                            RegisterCode = sd.RegisterCode
                        }
                    );

                foreach (var userSubject in userSubjects)
                {
                    UserSubjects.Add(userSubject);
                }
            }
        }

        public void LoadShareString(string shareString)
        {
            UserSubjects.Clear();
            if (!string.IsNullOrWhiteSpace(shareString))
            {
                var userSubjects = ShareString.GetSubjectFromShareString(shareString);
                if (userSubjects != null)
                {
                    foreach (var userSubject in userSubjects)
                    {
                        UserSubjects.Add(userSubject);
                    }
                    ImportCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private void CheckIsAvailableSession(UserSchedule session)
        {
            _courseCrawler.GetInfo(out var _, out var yearValue, out var _, out var semesterValue);
            if (session != null)
            {
                var semester = session.SemesterValue;
                var year = session.YearValue;
                var available = 1;
                var unavailable = 0;
                IsAvailableSession = semester.Equals(semesterValue, StringComparison.Ordinal) && year.Equals(yearValue, StringComparison.Ordinal) 
                    ? available 
                    : unavailable;
            }
            else
            {
                IsAvailableSession = -1;
            }
        }

        public void LoadScheduleSession()
        {
            ScheduleSessions.Clear();
            UserSubjects.Clear();
            var sessions = _unitOfWork.UserSchedules.GetAll();
            foreach (var session in sessions)
            {
                ScheduleSessions.Add(session);
            }
        }

        private void OnImport()
        {
            _eventAggregator.GetEvent<CloseDialogEvent>().Publish();
            _eventAggregator.GetEvent<ExitImportSubjectMsg>().Publish(UserSubjects.ToArray());
        }

        private void OnDelete()
        {
            var sessionName = SelectedScheduleSession.Name;
            var result = MessageBox.Show(
                  $"Bạn có chắc muốn xoá phiên {sessionName}?"
                , "Thông báo"
                , MessageBoxButton.YesNo
                , MessageBoxImage.Question
            );
            if (result == MessageBoxResult.Yes)
            {
                var removeResult = _unitOfWork.UserSchedules.Remove(SelectedScheduleSession);
                if (removeResult == 1)
                {
                    Reload();
                }
                else
                {
                    _ = MessageBox.Show(
                          "Có lỗi trong quá trình xử lý xoá phiên"
                        , "Thông báo"
                        , MessageBoxButton.OK
                        , MessageBoxImage.Error
                    );
                }
            }
        }

        private void Reload()
        {
            ScheduleSessions.Clear();
            UserSubjects.Clear();
            LoadScheduleSession();
        }
    }
}
