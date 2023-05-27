using CommunityToolkit.Mvvm.Input;

using Cs4rsa.BaseClasses;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;
using Cs4rsa.Services.CourseSearchSvc.Crawlers;
using Cs4rsa.Services.SubjectCrawlerSvc.Models;
using Cs4rsa.Utils;

using MaterialDesignThemes.Wpf;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Cs4rsa.Dialogs.Implements
{
    /// <summary>
    /// Mô tả:
    ///     Hộp thoại lưu bộ lịch mà người dùng đã sắp xếp.
    /// </summary>
    public class SaveSessionViewModel : ViewModelBase
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value ?? string.Empty;
                SaveCommand.NotifyCanExecuteChanged();
            }
        }

        public IEnumerable<ClassGroupModel> ClassGroupModels { get; set; }
        public RelayCommand SaveCommand { get; set; }

        private readonly IUnitOfWork _unitOfWork;
        private readonly CourseCrawler _courseCrawler;
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;
        private readonly ShareString _shareString;

        public SaveSessionViewModel(
            IUnitOfWork unitOfWork,
            CourseCrawler courseCrawler,
            ISnackbarMessageQueue snackbarMessageQueue,
            ShareString shareString)
        {
            _unitOfWork = unitOfWork;
            _courseCrawler = courseCrawler;
            _snackbarMessageQueue = snackbarMessageQueue;
            _shareString = shareString;
            _name = string.Empty;

            SaveCommand = new RelayCommand(Save, () => _name.Length > 0);
        }

        // 
        // Mô tả:
        //     Lưu bộ lịch đã sắp xếp.
        //     
        // 1. Với các lớp chỉ có một base school class duy nhất, chọn lớp này.
        // 2. Với các lớp có LAB, tức sẽ có một base class và một lớp (thường là LAB), sẽ chọn lớp này.
        // 3. Với các special class group, chọn lớp khác base class.
        // 
        private void Save()
        {
            List<ScheduleDetail> sessionDetails = _shareString
                .ConvertToUserSubjects(ClassGroupModels)
                .Select(us => new ScheduleDetail()
                {
                    SubjectCode = us.SubjectCode,
                    SubjectName = us.SubjectName,
                    ClassGroup = us.ClassGroup,
                    SelectedSchoolClass = us.SchoolClass,
                    RegisterCode = us.RegisterCode
                }).ToList();
            UserSchedule session = new()
            {
                Name = Name.Trim(),
                SaveDate = DateTime.Now,
                SemesterValue = _courseCrawler.CurrentSemesterValue,
                YearValue = _courseCrawler.CurrentYearValue,
                SessionDetails = sessionDetails
            };

            _unitOfWork.UserSchedules.Add(session);
            CloseDialog();
            string message = $"Đã lưu phiên hiện tại với tên {Name}";
            _snackbarMessageQueue.Enqueue(message);
        }
    }
}
