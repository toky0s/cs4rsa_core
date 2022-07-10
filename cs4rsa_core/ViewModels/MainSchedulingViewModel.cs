using CourseSearchService.Crawlers.Interfaces;

using cs4rsa_core.BaseClasses;
using cs4rsa_core.Messages;
using cs4rsa_core.ViewModels.Interfaces;

using LightMessageBus;
using LightMessageBus.Interfaces;

using Microsoft.Toolkit.Mvvm.Input;

namespace cs4rsa_core.ViewModels
{
    public class MainSchedulingViewModel : ViewModelBase,
        IMessageHandler<SubjectItemChangeMessage>,
        IMainSchedulingViewModel
    {
        #region Fields
        private string _currentYearInfo;
        private string _currentSemesterInfo;
        #endregion

        #region Bindings
        public string CurrentYearInfo
        {
            get => _currentYearInfo;
            set
            {
                _currentYearInfo = value;
                OnPropertyChanged();
            }
        }
        public string CurrentSemesterInfo
        {
            get => _currentSemesterInfo;
            set
            {
                _currentSemesterInfo = value;
                OnPropertyChanged();
            }
        }

        private int _totalCredit;
        public int TotalCredit
        {
            get => _totalCredit;
            set
            {
                _totalCredit = value;
                OnPropertyChanged();
            }
        }

        private int _totalSubject;
        public int TotalSubject
        {
            get => _totalSubject;
            set
            {
                _totalSubject = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Commands
        public RelayCommand OpenSettingCommand { get; set; }
        public RelayCommand OpenAutoScheduling { get; set; }
        #endregion

        public MainSchedulingViewModel(ICourseCrawler courseCrawler)
        {
            MessageBus.Default.FromAny().Where<SubjectItemChangeMessage>().Notify(this);

            CurrentSemesterInfo = courseCrawler.GetCurrentSemesterInfo();
            CurrentYearInfo = courseCrawler.GetCurrentYearInfo();

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
