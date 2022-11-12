using cs4rsa_core.BaseClasses;
using cs4rsa_core.Dialogs.DialogResults;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using cs4rsa_core.Cs4rsaDatabase.Interfaces;
using cs4rsa_core.Cs4rsaDatabase.Models;
using cs4rsa_core.Services.SubjectCrawlerSvc.Models;
using cs4rsa_core.Services.CourseSearchSvc.Crawlers.Interfaces;

namespace cs4rsa_core.Dialogs.Implements
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
        public SaveSessionViewModel(IUnitOfWork unitOfWork, ICourseCrawler courseCrawler)
        {
            _unitOfWork = unitOfWork;
            _courseCrawler = courseCrawler;
            ScheduleSessions = new();
            SaveCommand = new AsyncRelayCommand(Save);
        }

        public async Task LoadScheduleSessions()
        {
            ScheduleSessions.Clear();
            IEnumerable<UserSchedule> sessions = await _unitOfWork.Sessions.GetAllAsync();
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
            List<ScheduleDetail> sessionDetails = new();
            foreach (ClassGroupModel classGroupModel in ClassGroupModels)
            {
                string selectedSchoolClassName = classGroupModel.Get
                ScheduleDetail sessionDetail = new()
                {
                    SubjectCode = classGroupModel.SubjectCode,
                    ClassGroup = classGroupModel.ClassGroup.Name,
                    SubjectName = (await _unitOfWork.Keywords.GetKeyword(classGroupModel.SubjectCode)).SubjectName,
                    RegisterCode = classGroupModel.CurrentSchoolClassName,
                    SelectedSchoolClass = 
                };
                sessionDetails.Add(sessionDetail);
            }

            UserSchedule session = new()
            {
                Name = Name,
                SaveDate = DateTime.Now,
                SemesterValue = _courseCrawler.GetCurrentSemesterValue(),
                YearValue = _courseCrawler.GetCurrentYearValue(),
                SessionDetails = sessionDetails
            };

            await _unitOfWork.Sessions.AddAsync(session);
            await _unitOfWork.CompleteAsync();
            SaveResult result = new() { Name = Name };
            CloseDialogCallback.Invoke(result);
        }
    }
}
