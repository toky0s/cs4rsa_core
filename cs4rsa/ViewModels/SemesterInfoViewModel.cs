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
                RaisePropertyChanged();
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
                RaisePropertyChanged();
            }
        }

        public SemesterInfoViewModel()
        {
            HomeCourseSearch homeCourseSearch = new HomeCourseSearch();
            CurrentSemesterInfo = homeCourseSearch.CurrentSemesterInfo;
            CurrentYearInfo = homeCourseSearch.CurrentYearInfo;
        }
    }
}
