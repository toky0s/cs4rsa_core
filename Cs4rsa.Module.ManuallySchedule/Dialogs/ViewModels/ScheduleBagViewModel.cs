using Cs4rsa.Database.Interfaces;
using Cs4rsa.Database.Models;
using Cs4rsa.Module.ManuallySchedule.Dialogs.Models;

using Prism.Events;
using Prism.Mvvm;

using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Cs4rsa.Module.ManuallySchedule.Dialogs.ViewModels
{
    public class ScheduleBagViewModel : BindableBase
    {
        public ObservableCollection<ScheduleBagModel> ScheduleBagModels { get; set; }

        private readonly IEventAggregator _eventAggregator;
        private readonly IUnitOfWork _unitOfWork;

        public ScheduleBagViewModel(IEventAggregator eventAggregator, IUnitOfWork unitOfWork)
        {
            _eventAggregator = eventAggregator;
            _unitOfWork = unitOfWork;
            ScheduleBagModels = new ObservableCollection<ScheduleBagModel>();
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

        public void GetScheduleDetails(ScheduleBagModel scheduleBagModel)
        {
            var scheduleDetails = _unitOfWork.UserSchedules.GetSessionDetails(scheduleBagModel.UserScheduleId);
            foreach (var item in scheduleDetails.Select(sd => new ScheduleBagItemModel() { ClassGroup = sd.ClassGroup, RegisterCode = sd.RegisterCode, ScheduleDetailId = sd.ScheduleDetailId, SelectedSchoolClass = sd.SelectedSchoolClass, SubjectCode = sd.SubjectCode, SubjectName = sd.SubjectName }))
            {
                scheduleBagModel.ScheduleBagItemModels.Add(item);
            }
        }
    }
}
