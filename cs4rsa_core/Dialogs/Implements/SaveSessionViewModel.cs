using cs4rsa_core.BaseClasses;
using cs4rsa_core.Dialogs.DialogResults;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using cs4rsa_core.Cs4rsaDatabase.Interfaces;
using cs4rsa_core.Cs4rsaDatabase.Models;
using cs4rsa_core.Services.SubjectCrawlerSvc.Models;
using cs4rsa_core.Services.CourseSearchSvc.Crawlers.Interfaces;
using cs4rsa_core.Utils;

namespace cs4rsa_core.Dialogs.Implements
{
    public class SaveSessionViewModel : ViewModelBase
    {
        public IEnumerable<ClassGroupModel> ClassGroupModels { get; set; }
        public ObservableCollection<Session> ScheduleSessions { get; set; }
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
            IEnumerable<Session> sessions = await _unitOfWork.Sessions.GetAllAsync();
            foreach (Session session in sessions)
            {
                ScheduleSessions.Add(session);
            }
        }

        private async Task Save()
        {
            List<SessionDetail> sessionDetails = (await _shareString.ConvertToUserSubjects(ClassGroupModels))
                                                    .Select(us => new SessionDetail()
                                                    {
                                                        SubjectCode = us.SubjectCode,
                                                        SubjectName = us.SubjectName,
                                                        ClassGroup = us.ClassGroup,
                                                        SelectedSchoolClass = us.SchoolClass,
                                                        RegisterCode = us.RegisterCode
                                                    }).ToList();
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
