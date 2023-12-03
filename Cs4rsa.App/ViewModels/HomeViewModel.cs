using Cs4rsa.Database;
using Cs4rsa.Database.Interfaces;
using Cs4rsa.Service.CourseCrawler.Interfaces;

using Prism.Commands;
using Prism.Mvvm;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Cs4rsa.App.ViewModels
{
    public class HomeViewModel : BindableBase
    {
        private string _semesterInfo;
        public string SemesterInfo
        {
            get { return _semesterInfo; }
            set { SetProperty(ref _semesterInfo, value); }
        }

        private string _yearInfo;
        public string YearInfo
        {
            get { return _yearInfo; }
            set { SetProperty(ref _yearInfo, value); }
        }

        private string _yearValue;
        public string YearValue
        {
            get { return _yearValue; }
            set { SetProperty(ref _yearValue, value); }
        }

        private bool _isNewSemester;
        public bool IsNewSemester
        {
            get { return _isNewSemester; }
            set { SetProperty(ref _isNewSemester, value); }
        }

        public HomeViewModel(ICourseCrawler courseCrawler, IUnitOfWork unitOfWork)
        {
            courseCrawler.GetInfo(
                out string yearInfo, 
                out string yearValue, 
                out string semesterInfo, 
                out string semesterValue
            );

            IsNewSemester = (semesterValue != null && yearValue != null) 
                && (unitOfWork.Settings.GetByKey(DbConsts.StCurrentSemesterValue) != semesterValue 
                    || unitOfWork.Settings.GetByKey(DbConsts.StCurrentYearValue) != yearValue);

            YearInfo = yearInfo;
            SemesterInfo = semesterInfo;
        }
    }
}
