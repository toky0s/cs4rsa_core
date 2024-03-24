using Cs4rsa.Database.Interfaces;
using Cs4rsa.Module.ManuallySchedule.Dialogs.Models;
using Cs4rsa.Module.ManuallySchedule.Events;
using Cs4rsa.Service.Dialog.Events;

using Prism.Commands;
using Prism.Events;

using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Cs4rsa.Module.Shared;
using Cs4rsa.Module.ManuallySchedule.Dialogs.Views;
using Newtonsoft.Json;
using Cs4rsa.Infrastructure.Common;
using System;
using Cs4rsa.Database.Models;
using System.Collections.Generic;
using Cs4rsa.Database;

namespace Cs4rsa.Module.ManuallySchedule.Dialogs.ViewModels
{
    public class ScheduleBagViewModel : ShowDialogViewModelAbstract
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _currSemesterValue;

        private string _scheduleBagCode;
        public string ScheduleBagCode
        {
            get { return _scheduleBagCode; }
            set 
            { 
                SetProperty(ref _scheduleBagCode, value);
                ValidateCode(_scheduleBagCode);
            }
        }

        private bool _isCodeValid;
        public bool IsCodeValid
        {
            get { return _isCodeValid; }
            set { SetProperty(ref _isCodeValid, value); }
        }

        private ScheduleBagModel _enteredScheduleBagModel;
        public ScheduleBagModel EnteredScheduleBagModel
        {
            get { return _enteredScheduleBagModel; }
            set { SetProperty(ref _enteredScheduleBagModel, value); }
        }

        public ObservableCollection<ScheduleBagModel> ScheduleBagModels { get; set; }
        public DelegateCommand<ScheduleBagModel> ImportCommand { get; set; }
        public DelegateCommand<ScheduleBagModel> CopyCodeCommand { get; set; }
        public DelegateCommand<ScheduleBagModel> DeleteCommand { get; set; }
        public DelegateCommand<ScheduleBagModel> GetDetailsCommand { get; set; }
        public DelegateCommand SaveAndLoadCommand { get; set; }
        public ScheduleBagViewModel(IEventAggregator eventAggregator, IUnitOfWork unitOfWork)
        {
            _eventAggregator = eventAggregator;
            _unitOfWork = unitOfWork;
            _currSemesterValue = _unitOfWork.Settings.GetByKey(DbConsts.StCurrentSemesterValue);
            ScheduleBagModels = new ObservableCollection<ScheduleBagModel>();
            ImportCommand = new DelegateCommand<ScheduleBagModel>(ExecuteImportCommand);
            CopyCodeCommand = new DelegateCommand<ScheduleBagModel>(ExecuteCopyCodeCommand);
            GetDetailsCommand = new DelegateCommand<ScheduleBagModel>(ExecuteGetDetailsCommand);
            DeleteCommand = new DelegateCommand<ScheduleBagModel>(ExecuteDeleteCommand);
            SaveAndLoadCommand = new DelegateCommand(ExecuteSaveAndLoadCommand, CanExecuteSaveAndLoadCommand);
        }

        private void EnteredScheduleBagModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SaveAndLoadCommand.RaiseCanExecuteChanged();
        }

        private bool CanExecuteSaveAndLoadCommand()
        {
            return IsCodeValid // 1. Mã hợp lệ
                && !string.IsNullOrWhiteSpace(EnteredScheduleBagModel.Name) // 2. Tên không được để trống
                && !EnteredScheduleBagModel.IsExpired; // 3. Chưa hết hạn
        }

        private void ExecuteSaveAndLoadCommand()
        {
            // 1. Convert ScheduleBagModel thành UserSchedule
            UserSchedule userSchedule = new UserSchedule()
            {
                Name = EnteredScheduleBagModel.Name,
                SaveDate = DateTime.Now,
                Semester = EnteredScheduleBagModel.Semester,
                SemesterValue = EnteredScheduleBagModel.SemesterValue,
                Year = EnteredScheduleBagModel.Year,
                YearValue = EnteredScheduleBagModel.YearValue,
                SessionDetails = new List<ScheduleDetail>()
            };

            var scheduleDetails = EnteredScheduleBagModel.ScheduleBagItemModels.Select(sbi => new ScheduleDetail()
            {
                ClassGroup = sbi.ClassGroup,
                RegisterCode = sbi.RegisterCode,
                SelectedSchoolClass = sbi.SelectedSchoolClass,
                SubjectCode = sbi.SubjectCode,
                SubjectName = sbi.SubjectName
            });

            userSchedule.SessionDetails.AddRange(scheduleDetails);

            // 2. Lưu vào DB
            _unitOfWork.UserSchedules.Add(userSchedule);

            // 3. Convert thành UserSubjects
            var userSubjects = EnteredScheduleBagModel.ScheduleBagItemModels.Select(sbi => new UserSubject()
            {
                SubjectName = sbi.SubjectName,
                ClassGroup = sbi.ClassGroup,
                RegisterCode = sbi.RegisterCode,
                SchoolClass = sbi.SelectedSchoolClass,
                SubjectCode = sbi.SubjectCode
            });

            // 4. Đóng dialog và load lên màn hình
            _eventAggregator.GetEvent<CloseDialogEvent>().Publish();
            _eventAggregator.GetEvent<ExitImportSubjectMsg>().Publish(userSubjects);
        }

        private void ExecuteDeleteCommand(ScheduleBagModel model)
        {
            YesNoUc dialog = new YesNoUc(
                caption: "Bạn có chắc muốn xoá?",
                description: "Bộ lịch đã xoá không thể hoàn tác.",
                leftBtnContent: "CHẮC",
                rightBtnContent: "KHÔNG"
            )
            {
                LeftAction = () => DoDelete(model),
                RightAction = () => { IsOpen = false; }
            };

            Dialog = dialog;
            IsCloseOnClickAway = true;
            IsOpen = true;
        }

        private void DoDelete(ScheduleBagModel model)
        {
            IsOpen = false;
            _unitOfWork.UserSchedules.Remove(model.UserScheduleId);
            var idx = ScheduleBagModels.IndexOf(model);
            ScheduleBagModels.RemoveAt(idx);
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
                    IsExpired = !us.SemesterValue.Equals(_currSemesterValue)
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

                var json = JsonConvert.SerializeObject(scheduleBagModel);
                var encodeTo64Json = StringHelper.EncodeTo64(json);

                Clipboard.SetText(encodeTo64Json);
                SnackBarMessageQueue.Enqueue("Sao chép thành công");
            }
            catch
            {
                SnackBarMessageQueue.Enqueue("Sao chép không thành công");
            }
        }

        private void ValidateCode(string scheduleBagCode)
        {
            if (EnteredScheduleBagModel is object)
            {
                EnteredScheduleBagModel.PropertyChanged -= EnteredScheduleBagModel_PropertyChanged;
            }

            if (string.IsNullOrWhiteSpace(scheduleBagCode))
            {
                IsCodeValid = false;
            }
            else
            {
                try
                {
                    var json = StringHelper.DecodeFrom64(scheduleBagCode);
                    EnteredScheduleBagModel = JsonConvert.DeserializeObject<ScheduleBagModel>(json);
                    // Kiểm tra hết hạn
                    EnteredScheduleBagModel.IsExpired = !EnteredScheduleBagModel.Semester.Equals(_currSemesterValue);
                    EnteredScheduleBagModel.PropertyChanged += EnteredScheduleBagModel_PropertyChanged;
                    SaveAndLoadCommand.RaiseCanExecuteChanged();
                    IsCodeValid = true;
                }
                catch
                {
                    IsCodeValid = false;
                }
            }
        }
    }
}
