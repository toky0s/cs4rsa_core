using Cs4rsa.Database.Interfaces;
using Cs4rsa.Database.Models;
using Cs4rsa.Module.ManuallySchedule.Dialogs.Models;
using Cs4rsa.Service.Dialog;
using Cs4rsa.Service.Dialog.Interfaces;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsa.Module.ManuallySchedule.Dialogs.ViewModels
{
    public class ScheduleDetailUCViewModel : DialogViewModelBase
    {
        private UserSchedule _userSchedule;
        public UserSchedule UserSchedule
        {
            get { return _userSchedule; }
            set { SetProperty(ref _userSchedule, value); LoadScheduleDetail(value); }
        }

        private void LoadScheduleDetail(UserSchedule value)
        {
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

        public ObservableCollection<UserSubject> UserSubjects { get; set; }

        private readonly IDialogService _dialogService;
        private readonly IUnitOfWork _unitOfWork;

        public ScheduleDetailUCViewModel(
            IDialogService dialogService,
            IUnitOfWork unitOfWork) : base("View Details")
        {
            _dialogService = dialogService;
            _unitOfWork = unitOfWork;

            UserSubjects = new ObservableCollection<UserSubject>();
        }
    }
}
