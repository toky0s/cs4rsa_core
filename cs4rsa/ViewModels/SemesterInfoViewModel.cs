using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using cs4rsa.Crawler;
using cs4rsa.BaseClasses;

namespace cs4rsa.ViewModels
{
    public class SemesterInfoViewModel: NotifyPropertyChangedBase
    {
        private string currentYearInfo;
        private string currentSemesterInfo;

        public string CurrentYearInfo
        {
            get
            {
                return currentYearInfo;
            }
            set
            {
                currentYearInfo = value;
                RaisePropertyChanged("CurrentYearInfo");
            }
        }

        public string CurrentSemesterInfo
        {
            get
            {
                return currentSemesterInfo;
            }
            set
            {
                currentSemesterInfo = value;
                RaisePropertyChanged("CurrentSemesterInfo");
            }
        }

        public void LoadSemesterInfo()
        {
            HomeCourseSearch homeCourseSearch = new HomeCourseSearch();
            this.CurrentSemesterInfo = homeCourseSearch.CurrentSemesterInfo;
            this.CurrentYearInfo = homeCourseSearch.CurrentYearInfo;
        }
    }
}
