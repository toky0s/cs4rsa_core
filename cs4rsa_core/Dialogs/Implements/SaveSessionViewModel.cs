using CourseSearchService.Crawlers.Interfaces;
using cs4rsa_core.BaseClasses;
using cs4rsa_core.Dialogs.DialogResults;
using Cs4rsaDatabaseService.Interfaces;
using Cs4rsaDatabaseService.Models;
using CommunityToolkit.Mvvm.Input;
using SubjectCrawlService1.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace cs4rsa_core.Dialogs.Implements
{
    public class SaveSessionViewModel : ViewModelBase
    {
        public IEnumerable<ClassGroupModel> ClassGroupModels { get; set; }
        public ObservableCollection<Session> ScheduleSessions { get; set; }
        public string Name { get; set; }
        public bool IsSaveAsJsonFile { get; set; }
        public AsyncRelayCommand SaveCommand { get; set; }
        public Action<SaveResult> CloseDialogCallback { get; set; }

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICourseCrawler _courseCrawler;
        public SaveSessionViewModel(IUnitOfWork unitOfWork, ICourseCrawler courseCrawler)
        {
            _unitOfWork = unitOfWork;
            _courseCrawler = courseCrawler;
            IsSaveAsJsonFile = true;
            ScheduleSessions = new();
            SaveCommand = new AsyncRelayCommand(Save);
        }

        public async Task LoadScheduleSessions()
        {
            ScheduleSessions.Clear();
            IEnumerable<Session> sessions = await _unitOfWork.Sessions.GetAllAsync();
            foreach (Session session in sessions)
            {
                ScheduleSessions.Add(session);
            }
        }

        private async Task Save()
        {
            List<SessionDetail> sessionDetails = new();
            foreach (ClassGroupModel classGroupModel in ClassGroupModels)
            {
                List<SessionSchoolClass> sessionSchoolClasses = new();
                foreach (SchoolClassModel ssc in classGroupModel.GetSchoolClassModels())
                {
                    SessionSchoolClass sessionSchoolClass = new()
                    {
                        Name = ssc.SchoolClassName,
                        Type = ssc.Type.Code
                    };
                    sessionSchoolClasses.Add(sessionSchoolClass);
                }
                SessionDetail sessionDetail = new()
                {
                    SubjectCode = classGroupModel.SubjectCode,
                    ClassGroup = classGroupModel.ClassGroup.Name,
                    SubjectName = _unitOfWork.Keywords.GetKeyword(classGroupModel.SubjectCode).SubjectName,
                    SessionSchoolClasses = sessionSchoolClasses,
                    RegisterCode = classGroupModel.CurrentRegisterCode
                };
                sessionDetails.Add(sessionDetail);
            }

            Session session = new()
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
