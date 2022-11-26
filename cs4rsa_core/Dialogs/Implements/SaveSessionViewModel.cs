using CommunityToolkit.Mvvm.Input;

using Cs4rsa.BaseClasses;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;
using Cs4rsa.Dialogs.DialogResults;
using Cs4rsa.Services.CourseSearchSvc.Crawlers.Interfaces;
using Cs4rsa.Services.SubjectCrawlerSvc.Models;
using Cs4rsa.Utils;

using MaterialDesignThemes.Wpf;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public AsyncRelayCommand SaveCommand { get; set; }

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICourseCrawler _courseCrawler;
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;
        private readonly ShareString _shareString;

        public SaveSessionViewModel(
            IUnitOfWork unitOfWork,
            ICourseCrawler courseCrawler,
            ISnackbarMessageQueue snackbarMessageQueue,
            ShareString shareString)
        {
            _unitOfWork = unitOfWork;
            _courseCrawler = courseCrawler;
            _snackbarMessageQueue = snackbarMessageQueue;
            _shareString = shareString;
            _name = string.Empty;

            SaveCommand = new AsyncRelayCommand(Save, () => _name.Length > 0);
        }

        // 
        // Mô tả:
        //     Lưu bộ lịch đã sắp xếp.
        //     
        // 1. Với các lớp chỉ có một base school class duy nhất, chọn lớp này.
        // 2. Với các lớp có LAB, tức sẽ có một base class và một lớp (thường là LAB), sẽ chọn lớp này.
        // 3. Với các special class group, chọn lớp khác base class.
        // 
        private async Task Save()
        {
            List<ScheduleDetail> sessionDetails = (await _shareString.ConvertToUserSubjects(ClassGroupModels))
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
                Name = Name,
                SaveDate = DateTime.Now,
                SemesterValue = _courseCrawler.GetCurrentSemesterValue(),
                YearValue = _courseCrawler.GetCurrentYearValue(),
                SessionDetails = sessionDetails
            };

            await _unitOfWork.UserSchedule.AddAsync(session);
            await _unitOfWork.CompleteAsync();
            SaveResult result = new() { Name = Name };
            CloseDialog();
            if (result != null)
            {
                string message = $"Đã lưu phiên hiện tại với tên {result.Name}";
                _snackbarMessageQueue.Enqueue(message);
            }
        }
    }
}
