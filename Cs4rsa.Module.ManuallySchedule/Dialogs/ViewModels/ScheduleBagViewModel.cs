using Cs4rsa.Database.Interfaces;
using Cs4rsa.Module.ManuallySchedule.Dialogs.Models;
using Cs4rsa.Module.ManuallySchedule.Events;
using Cs4rsa.Module.ManuallySchedule.Utils;
using Cs4rsa.Service.Dialog.Events;
using Cs4rsa.Infrastructure.Events;

using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Cs4rsa.Module.ManuallySchedule.Dialogs.ViewModels
{
    public class ScheduleBagViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ShareString _shareString;

        public ObservableCollection<ScheduleBagModel> ScheduleBagModels { get; set; }
        public DelegateCommand<ScheduleBagModel> ImportCommand { get; set; }
        public DelegateCommand<ScheduleBagModel> CopyCodeCommand { get; set; }
        public DelegateCommand<ScheduleBagModel> GetDetailsCommand { get; set; }
        public ScheduleBagViewModel(IEventAggregator eventAggregator, IUnitOfWork unitOfWork, ShareString shareStringSvc)
        {
            _eventAggregator = eventAggregator;
            _unitOfWork = unitOfWork;
            _shareString = shareStringSvc;
            ScheduleBagModels = new ObservableCollection<ScheduleBagModel>();
            ImportCommand = new DelegateCommand<ScheduleBagModel>(ExecuteImportCommand);
            CopyCodeCommand = new DelegateCommand<ScheduleBagModel>(ExecuteCopyCodeCommand);
            GetDetailsCommand = new DelegateCommand<ScheduleBagModel>(ExecuteGetDetailsCommand);
        }

        private void ExecuteImportCommand(ScheduleBagModel payload)
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
            var items = userSchedules
                .Select(us => new ScheduleBagModel()
                {
                    ScheduleBagItemModels = new ObservableCollection<ScheduleBagItemModel>(),
                    Name = us.Name,
                    SaveDate = us.SaveDate,
                    SemesterValue = us.SemesterValue,
                    YearValue = us.YearValue,
                    Year = us.Year,
                    Semester = us.Semester,
                    UserScheduleId = us.UserScheduleId,
                });
            ScheduleBagModels.AddRange(items);
        }

        public void ExecuteGetDetailsCommand(ScheduleBagModel scheduleBagModel)
        {
            if (scheduleBagModel.ScheduleBagItemModels.Count == 0)
            {
                var scheduleBagItemModels = _unitOfWork.UserSchedules
                    .GetSessionDetails(scheduleBagModel.UserScheduleId)
                    .Select(sd => new ScheduleBagItemModel()
                    {
                        ClassGroup = sd.ClassGroup,
                        RegisterCode = sd.RegisterCode,
                        ScheduleDetailId = sd.ScheduleDetailId,
                        SelectedSchoolClass = sd.SelectedSchoolClass,
                        SubjectCode = sd.SubjectCode,
                        SubjectName = sd.SubjectName
                    });
                scheduleBagModel.ScheduleBagItemModels.AddRange(scheduleBagItemModels);
            }
        }

        private void ExecuteCopyCodeCommand(ScheduleBagModel scheduleBagModel)
        {
            try
            {
                // Load ScheduleBagItemModels nếu nó chưa được load từ database
                if (scheduleBagModel.ScheduleBagItemModels.Count == 0)
                {
                    var scheduleBagItemModels = _unitOfWork.UserSchedules
                    .GetSessionDetails(scheduleBagModel.UserScheduleId)
                    .Select(sd => new ScheduleBagItemModel()
                    {
                        ClassGroup = sd.ClassGroup,
                        RegisterCode = sd.RegisterCode,
                        ScheduleDetailId = sd.ScheduleDetailId,
                        SelectedSchoolClass = sd.SelectedSchoolClass,
                        SubjectCode = sd.SubjectCode,
                        SubjectName = sd.SubjectName
                    });
                    scheduleBagModel.ScheduleBagItemModels.AddRange(scheduleBagItemModels);
                }

                var userSubjects = scheduleBagModel.ScheduleBagItemModels.Select(sbi => new UserSubject()
                {
                    SubjectName = sbi.SubjectName,
                    ClassGroup = sbi.ClassGroup,
                    RegisterCode = sbi.RegisterCode,
                    SchoolClass = sbi.SelectedSchoolClass,
                    SubjectCode = sbi.SubjectCode
                });
                string shareString = _shareString.GetShareString(userSubjects);

                Clipboard.SetText(shareString);
                _eventAggregator.GetEvent<SnackbarMsgEvent>().Publish("Sao chép thành công");
            }
            catch
            {
                _eventAggregator.GetEvent<SnackbarMsgEvent>().Publish("Sao chép không thành công");
            }
        }
    }
}
