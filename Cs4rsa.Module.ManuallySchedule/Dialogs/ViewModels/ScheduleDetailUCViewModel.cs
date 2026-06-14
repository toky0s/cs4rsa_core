using Cs4rsa.Database.Interfaces;
using Cs4rsa.Database.Models;
using Cs4rsa.Module.ManuallySchedule.Dialogs.Models;
using Cs4rsa.Module.ManuallySchedule.Utils;
using Cs4rsa.Service.CourseCrawler.Interfaces;
using Cs4rsa.Service.Dialog;
using Cs4rsa.Service.Dialog.Interfaces;

using Microsoft.Extensions.Logging;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsa.Module.ManuallySchedule.Dialogs.ViewModels
{
    public class ScheduleDetailUCViewModel : BindableBase, IDialogAware
    {
        private bool _isShareString;
        /// <summary>
        /// Thông báo rằng popup được mở bằng Share string.
        /// </summary>
        public bool IsShareString
        {
            get { return _isShareString; }
            set { SetProperty(ref _isShareString, value); }
        }

        private UserSchedule _userSchedule;
        public UserSchedule UserSchedule
        {
            get { return _userSchedule; }
            set { SetProperty(ref _userSchedule, value); LoadScheduleDetail(value); }
        }

        private void LoadScheduleDetail(UserSchedule value)
        {
            LoadCommand.RaiseCanExecuteChanged();
            LoadUserSubject(value);
        }

        private void LoadUserSubject(UserSchedule userSchedule)
        {
            if (userSchedule != null)
            {
                UserSubjects.Clear();
                var userSubjects = _unitOfWork.UserSchedules
                    .GetSessionDetails(userSchedule.UserScheduleId)
                    .Select(
                        sd =>
                        {
                            var us = new UserSubject()
                            {
                                SubjectCode = sd.SubjectCode,
                                SubjectName = sd.SubjectName,
                                ClassGroup = sd.ClassGroup,
                                SchoolClass = sd.SelectedSchoolClass,
                                RegisterCode = sd.RegisterCode,
                            };
                            CheckStatus(us);
                            return us;
                        }
                    ).ToList();
                UserSubjects.AddRange(userSubjects);
            }
        }

        #region Commands
        private DelegateCommand _loadMergeCommand;
        public DelegateCommand LoadMergeCommand =>
            _loadMergeCommand ?? (_loadMergeCommand = new DelegateCommand(ExecuteLoadMergeCommand, CanExecuteLoadMergeCommand));

        void ExecuteLoadMergeCommand()
        {
            if (IsShareString)
            {
                _logger.LogInformation("Load and merge schedule detail for share string");
            }
            else
            {
                _logger.LogInformation("Load and merge schedule detail for schedule {UserScheduleId} - {UserScheduleName}", UserSchedule.UserScheduleId, UserSchedule.Name);
            }
            var parameters = new DialogParameters
                {
                    { "UserSubjects", UserSubjects },
                    { "Action", "Merge" },
                };
            RequestClose.Invoke(new DialogResult(ButtonResult.OK, parameters));
        }

        bool CanExecuteLoadMergeCommand()
        {
            return true;
        }

        private DelegateCommand _loadCommand;
        public DelegateCommand LoadCommand =>
            _loadCommand ?? (_loadCommand = new DelegateCommand(ExecuteLoadCommand, CanExecuteLoadCommand));

        void ExecuteLoadCommand()
        {
            if (IsShareString)
            {
                _logger.LogInformation("Load schedule detail for share string");
            }
            else
            {
                _logger.LogInformation("Load schedule detail for schedule {UserScheduleId} - {UserScheduleName}", UserSchedule.UserScheduleId, UserSchedule.Name);
            }
            var parameters = new DialogParameters
                {
                    { "UserSubjects", UserSubjects },
                    { "Action", "Overwrite" },
                };
            RequestClose.Invoke(new DialogResult(ButtonResult.OK, parameters));
        }

        bool CanExecuteLoadCommand()
        {
            return true;
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            UserSchedule = parameters.GetValue<UserSchedule>("UserSchedule");
            var arrUserSubjects = parameters.GetValue<UserSubject[]>("UserSubjects");
            if (arrUserSubjects != null)
            {
                IsShareString = true;
                UserSubjects.Clear();
                var updatedUserSubjects = arrUserSubjects.Select(CheckStatus).ToList();
                UserSubjects.AddRange(updatedUserSubjects);
            }
            else
            {
                IsShareString = false;
            }

            LoadCommand.RaiseCanExecuteChanged();
        }

        private UserSubject CheckStatus(UserSubject userSubject)
        {
            var subject = _unitOfWork.Keywords.GetKeywordBySubjectCode(userSubject.SubjectCode);
            if (subject != null)
            {
                userSubject.Status = "OK";
            }
            else
            {
                userSubject.Status = "NOT OK";
            }
            return userSubject;
        }
        #endregion

        public ObservableCollection<UserSubject> UserSubjects { get; set; }

        public string Title => "View Schedule Details";

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ScheduleDetailUCViewModel> _logger;

        public event Action<IDialogResult> RequestClose;

        public ScheduleDetailUCViewModel(
            ILogger<ScheduleDetailUCViewModel> logger,
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;

            UserSubjects = new ObservableCollection<UserSubject>();
        }
    }
}
