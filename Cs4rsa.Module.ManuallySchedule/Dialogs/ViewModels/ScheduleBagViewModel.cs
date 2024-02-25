using Cs4rsa.Database.Interfaces;
using Cs4rsa.Infrastructure.Common;
using Cs4rsa.Module.ManuallySchedule.Dialogs.Models;
using Cs4rsa.Module.ManuallySchedule.Events;
using Cs4rsa.Module.ManuallySchedule.Models;
using Cs4rsa.Service.Dialog.Events;

using Newtonsoft.Json;

using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Cs4rsa.Module.ManuallySchedule.Dialogs.ViewModels
{
    public class ScheduleBagViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IUnitOfWork _unitOfWork;

        public ObservableCollection<ScheduleBagModel> ScheduleBagModels { get; set; }
        public DelegateCommand<ScheduleBagModel> ImportCommand { get; set; }
        public DelegateCommand<ScheduleBagModel> GetDetailsCommand { get; set; }
        public ScheduleBagViewModel(IEventAggregator eventAggregator, IUnitOfWork unitOfWork)
        {
            _eventAggregator = eventAggregator;
            _unitOfWork = unitOfWork;
            ScheduleBagModels = new ObservableCollection<ScheduleBagModel>();
            ImportCommand = new DelegateCommand<ScheduleBagModel>(ExecuteImportCommand);
            GetDetailsCommand = new DelegateCommand<ScheduleBagModel>(ExecuteGetDetailsCommand);
        }

        void ExecuteImportCommand(ScheduleBagModel payload)
        {
            if (payload.ScheduleBagItemModels.Count == 0)
            {
                var scheduleDetails = _unitOfWork.UserSchedules.GetSessionDetails(payload.UserScheduleId);
                foreach (var item in scheduleDetails.Select(sd => new ScheduleBagItemModel() { ClassGroup = sd.ClassGroup, RegisterCode = sd.RegisterCode, ScheduleDetailId = sd.ScheduleDetailId, SelectedSchoolClass = sd.SelectedSchoolClass, SubjectCode = sd.SubjectCode, SubjectName = sd.SubjectName }))
                {
                    payload.ScheduleBagItemModels.Add(item);
                }
            }

            var userSubjects = payload.ScheduleBagItemModels.Select(sbi => new UserSubject()
            {
                SubjectName = sbi.SubjectName,
                ClassGroup = sbi.ClassGroup,
                RegisterCode = sbi.RegisterCode,
                SchoolClass = sbi.SelectedSchoolClass,
                SubjectCode = sbi.SubjectCode
            });

            _eventAggregator.GetEvent<CloseDialogEvent>().Publish();
            _eventAggregator.GetEvent<ExitImportSubjectMsg>().Publish(userSubjects);
        }

        public async Task LoadScheduleSession()
        {
            ScheduleBagModels.Clear();
            var userSchedules = await Task.Run(_unitOfWork.UserSchedules.GetAll);
            foreach (var item in userSchedules.Select(us => new ScheduleBagModel() { ScheduleBagItemModels = new ObservableCollection<ScheduleBagItemModel>(), Name = us.Name, SaveDate = us.SaveDate, SemesterValue = us.SemesterValue, YearValue = us.YearValue, UserScheduleId = us.UserScheduleId }))
            {
                ScheduleBagModels.Add(item);
            };
        }

        public void ExecuteGetDetailsCommand(ScheduleBagModel scheduleBagModel)
        {
            if (scheduleBagModel.ScheduleBagItemModels.Count == 0)
            {
                var scheduleDetails = _unitOfWork.UserSchedules.GetSessionDetails(scheduleBagModel.UserScheduleId);
                foreach (var item in scheduleDetails.Select(sd => new ScheduleBagItemModel() { ClassGroup = sd.ClassGroup, RegisterCode = sd.RegisterCode, ScheduleDetailId = sd.ScheduleDetailId, SelectedSchoolClass = sd.SelectedSchoolClass, SubjectCode = sd.SubjectCode, SubjectName = sd.SubjectName }))
                {
                    scheduleBagModel.ScheduleBagItemModels.Add(item);
                }
            }
        }
    }
}
