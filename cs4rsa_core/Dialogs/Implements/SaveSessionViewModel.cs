using System;
using System.Collections.Generic;
using cs4rsa_core.Dialogs.DialogResults;
using cs4rsa_core.Models;
using cs4rsa_core.BaseClasses;
using System.Collections.ObjectModel;
using Microsoft.Toolkit.Mvvm.Input;
using Cs4rsaDatabaseService.DataProviders;
using Cs4rsaDatabaseService.Models;
using CourseSearchService.Crawlers.Interfaces;
using System.Linq;
using Cs4rsaDatabaseService.Interfaces;

namespace cs4rsa_core.Dialogs.Implements
{
    class SaveSessionViewModel : ViewModelBase
    {
        public List<ClassGroupModel> ClassGroupModels { get; set; }

        public ObservableCollection<Session> ScheduleSessions { get; set; }

        private string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged();
                SaveCommand.NotifyCanExecuteChanged();
            }
        }

        public RelayCommand SaveCommand { get; set; }

        public RelayCommand CancelCommand { get; set; }

        public Action<SaveResult> CloseDialogCallback { get; set; }

        private IUnitOfWork _unitOfWork;
        private ICourseCrawler _courseCrawler;
        public SaveSessionViewModel(IUnitOfWork unitOfWork, ICourseCrawler courseCrawler)
        {
            _unitOfWork = unitOfWork;
            _courseCrawler = courseCrawler;
            ScheduleSessions = new();
            SaveCommand = new RelayCommand(Save, () => true);
            CancelCommand = new RelayCommand(Cancle, () => true);
            LoadScheduleSessions();
        }

        private void LoadScheduleSessions()
        {
            List<Session> sessions = _unitOfWork.Sessions.GetAll().ToList();
            sessions.ForEach(session => ScheduleSessions.Add(session));
        }

        private void Cancle()
        {
            CloseDialogCallback.Invoke(null);
        }

        private void Save()
        {
            List<SessionDetail> sessionDetails = new();
            foreach (ClassGroupModel classGroupModel in ClassGroupModels)
            {
                SessionDetail sessionDetail = new()
                {
                    SubjectCode = classGroupModel.SubjectCode,
                    ClassGroup = classGroupModel.ClassGroup.Name,
                    SubjectName = _unitOfWork.Keywords.GetKeyword(classGroupModel.SubjectCode).SubjectName
                };
                sessionDetails.Add(sessionDetail);
            }

            Session session = new()
            {
                Name = name,
                SaveDate = DateTime.Now,
                SemesterValue = _courseCrawler.GetCurrentSemesterValue(),
                YearValue = _courseCrawler.GetCurrentYearValue(),
                SessionDetails = sessionDetails
            };

            _unitOfWork.Sessions.Add(session);
            _unitOfWork.Complete();
            SaveResult result = new SaveResult() { Name = name };
            CloseDialogCallback.Invoke(result);
        }
    }
}
