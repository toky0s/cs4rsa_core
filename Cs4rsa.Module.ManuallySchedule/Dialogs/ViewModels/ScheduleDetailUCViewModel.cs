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
        private UserSchedule _userSchedule;
        public UserSchedule UserSchedule
        {
            get { return _userSchedule; }
            set { SetProperty(ref _userSchedule, value); LoadScheduleDetail(value); }
        }

        private bool _isAvailable;
        public bool IsAvailable
        {
            get { return _isAvailable; }
            set { SetProperty(ref _isAvailable, value); }
        }

        private void LoadScheduleDetail(UserSchedule value)
        {
            IsAvailable = ValidateUserSchedule.CheckIsAvailableSession(_courseCrawler, value);
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
                        sd => new UserSubject()
                        {
                            SubjectCode = sd.SubjectCode,
                            SubjectName = sd.SubjectName,
                            ClassGroup = sd.ClassGroup,
                            SchoolClass = sd.SelectedSchoolClass,
                            RegisterCode = sd.RegisterCode
                        }
                    );

                UserSubjects.AddRange(userSubjects);
            }
        }

        #region Commands
        private DelegateCommand _loadCommand;
        public DelegateCommand LoadCommand =>
            _loadCommand ?? (_loadCommand = new DelegateCommand(ExecuteLoadCommand, CanExecuteLoadCommand));

        void ExecuteLoadCommand()
        {
            _logger.LogInformation("Load schedule detail for schedule {UserScheduleId} - {UserScheduleName}", UserSchedule.UserScheduleId, UserSchedule.Name);
            RequestClose.Invoke(new DialogResult(ButtonResult.OK));
        }

        bool CanExecuteLoadCommand()
        {
            return IsAvailable;
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
        }
        #endregion

        public ObservableCollection<UserSubject> UserSubjects { get; set; }

        public string Title => "View Schedule Details";

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICourseCrawler _courseCrawler;
        private readonly ILogger<ScheduleDetailUCViewModel> _logger;

        public event Action<IDialogResult> RequestClose;

        public ScheduleDetailUCViewModel(
            ILogger<ScheduleDetailUCViewModel> logger,
            ICourseCrawler courseCrawler,
            IUnitOfWork unitOfWork)
        {
            _courseCrawler = courseCrawler;
            _unitOfWork = unitOfWork;
            _logger = logger;

            UserSubjects = new ObservableCollection<UserSubject>();
        }
    }
}
