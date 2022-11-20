using CommunityToolkit.Mvvm.Input;

using Cs4rsa.BaseClasses;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;
using Cs4rsa.Dialogs.DialogResults;
using Cs4rsa.Services.CourseSearchSvc.Crawlers.Interfaces;
using Cs4rsa.Services.SubjectCrawlerSvc.Models;
using Cs4rsa.Utils;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Cs4rsa.Dialogs.Implements
{
    /**
     * Mô tả:
     *      ViewModel thực hiện việc lưu thông tin bộ lịch
     *      mà người dùng đã sắp xếp
     */
    public class SaveSessionViewModel : ViewModelBase
    {
        public IEnumerable<ClassGroupModel> ClassGroupModels { get; set; }
        public ObservableCollection<UserSchedule> ScheduleSessions { get; set; }
        public string Name { get; set; }
        public AsyncRelayCommand SaveCommand { get; set; }
        public Action<SaveResult> CloseDialogCallback { get; set; }

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICourseCrawler _courseCrawler;
        private readonly ShareString _shareString;

        public SaveSessionViewModel(
            IUnitOfWork unitOfWork,
            ICourseCrawler courseCrawler,
            ShareString shareString)
        {
            _unitOfWork = unitOfWork;
            _courseCrawler = courseCrawler;
            _shareString = shareString;

            ScheduleSessions = new();
            SaveCommand = new AsyncRelayCommand(Save);
        }

        public async Task LoadScheduleSessions()
        {
            ScheduleSessions.Clear();
            IEnumerable<UserSchedule> sessions = await _unitOfWork.UserSchedule.GetAllAsync();
            foreach (UserSchedule session in sessions)
            {
                ScheduleSessions.Add(session);
            }
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
            CloseDialogCallback.Invoke(result);
        }
    }
}
