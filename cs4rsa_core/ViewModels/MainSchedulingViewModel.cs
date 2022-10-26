using cs4rsa_core.BaseClasses;
using cs4rsa_core.Messages.Publishers;
using cs4rsa_core.ViewModels.Interfaces;
using MaterialDesignThemes.Wpf;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using cs4rsa_core.Services.CourseSearchSvc.Crawlers.Interfaces;

namespace cs4rsa_core.ViewModels
{
    public class MainSchedulingViewModel : ViewModelBase,
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

        public MainSchedulingViewModel(ICourseCrawler courseCrawler, ISnackbarMessageQueue snackbarMessageQueue)
        {

            WeakReferenceMessenger.Default.Register<SearchVmMsgs.SubjectItemChangedMsg>(this, (r, m) =>
            {
                TotalCredit = m.Value.Item1;
                TotalSubject = m.Value.Item2;
            });

            CurrentSemesterInfo = courseCrawler.GetCurrentSemesterInfo();
            CurrentYearInfo = courseCrawler.GetCurrentYearInfo();

            TotalCredit = 0;
            TotalSubject = 0;
        }
    }
}
