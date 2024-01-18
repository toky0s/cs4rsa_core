using Cs4rsa.Database.Interfaces;
using Cs4rsa.Database.Models;
using Cs4rsa.Module.ManuallySchedule.Models;
using Cs4rsa.Module.ManuallySchedule.Utils;
using Cs4rsa.Service.Dialog.Interfaces;

using MaterialDesignThemes.Wpf;

using Prism.Commands;
using Prism.Mvvm;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Cs4rsa.Module.ManuallySchedule.Dialogs.ViewModels
{
    /// <summary>
    /// Hộp thoại lưu bộ lịch mà người dùng đã sắp xếp.
    /// </summary>
    public class SaveSessionUCViewModel : BindableBase
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public IEnumerable<ClassGroupModel> ClassGroupModels { get; set; }
        public DelegateCommand SaveCommand { get; set; }

        private readonly IUnitOfWork _unitOfWork;
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;
        private readonly ShareString _shareString;
        private readonly IDialogService _dialogService;

        public SaveSessionUCViewModel(
            IUnitOfWork unitOfWork,
            ISnackbarMessageQueue snackbarMessageQueue,
            IDialogService dialogService,
            ShareString shareString)
        {
            _unitOfWork = unitOfWork;
            _snackbarMessageQueue = snackbarMessageQueue;
            _shareString = shareString;
            _dialogService = dialogService;
            _name = string.Empty;

            var hs = snackbarMessageQueue.GetHashCode();

            SaveCommand = new DelegateCommand(Save, () => _name.Length > 0).ObservesProperty(() => Name);
        }

        /// <remarks>
        /// Lưu bộ lịch đã sắp xếp.
        /// 1. Với các lớp chỉ có một base school class duy nhất, chọn lớp này.
        /// 2. Với các lớp có LAB, tức sẽ có một base class và một lớp (thường là LAB), sẽ chọn lớp này.
        /// 3. Với các special class group, chọn lớp khác base class.
        /// </remarks>
        private void Save()
        {
            var sessionDetails = _shareString
                .ConvertToUserSubjects(ClassGroupModels)
                .Select(us => new ScheduleDetail()
                {
                    SubjectCode = us.SubjectCode,
                    SubjectName = us.SubjectName,
                    ClassGroup = us.ClassGroup,
                    SelectedSchoolClass = us.SchoolClass,
                    RegisterCode = us.RegisterCode
                }).ToList();

            var session = new UserSchedule()
            {
                Name = Name.Trim(),
                SaveDate = DateTime.Now,
                SemesterValue = _unitOfWork.Settings.GetByKey(Setting.SemesterValue),
                YearValue = _unitOfWork.Settings.GetByKey(Setting.YearValue),
                SessionDetails = sessionDetails
            };

            _unitOfWork.UserSchedules.Add(session);
            _dialogService.CloseDialog();
            _snackbarMessageQueue.Enqueue($"Đã lưu phiên hiện tại với tên {Name}");
        }
    }
}
