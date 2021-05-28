using cs4rsa.Crawler;
using cs4rsa.BaseClasses;
using LightMessageBus;
using LightMessageBus.Interfaces;
using cs4rsa.Messages;

namespace cs4rsa.ViewModels
{
    public class SemesterInfoViewModel: NotifyPropertyChangedBase, IMessageHandler<SubjectItemChangeMessage>
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

        private int _totalCredit = 0;
        public int TotalCredit
        {
            get
            {
                return _totalCredit;
            }
            set
            {
                _totalCredit = value;
                RaisePropertyChanged();
            }
        }

        private int _totalSubject = 0;
        public int TotalSubject
        {
            get
            {
                return _totalSubject;
            }
            set
            {
                _totalSubject = value;
                RaisePropertyChanged();
            }
        }

        public SemesterInfoViewModel()
        {
            HomeCourseSearch homeCourseSearch = new HomeCourseSearch();
            CurrentSemesterInfo = homeCourseSearch.CurrentSemesterInfo;
            CurrentYearInfo = homeCourseSearch.CurrentYearInfo;
            MessageBus.Default.FromAny().Where<SubjectItemChangeMessage>().Notify(this);
            TotalCredit = 0;
            TotalSubject = 0;
        }

        public void Handle(SubjectItemChangeMessage message)
        {
            TotalCredit = message.Source.TotalCredits;
            TotalSubject = message.Source.TotalSubject;
        }
    }
}
