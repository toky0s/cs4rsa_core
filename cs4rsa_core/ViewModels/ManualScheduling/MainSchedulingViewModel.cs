using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.BaseClasses;
using Cs4rsa.Messages.Publishers;
using Cs4rsa.Services.CourseSearchSvc.Crawlers.Interfaces;

namespace Cs4rsa.ViewModels.ManualScheduling
{
    internal partial class MainSchedulingViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _currentYearInfo;
        [ObservableProperty]
        private string _currentSemesterInfo;
        [ObservableProperty]
        private int _totalCredit;
        [ObservableProperty]
        private int _totalSubject;

        public MainSchedulingViewModel(ICourseCrawler courseCrawler)
        {

            Messenger.Register<SearchVmMsgs.SubjectItemChangedMsg>(this, (r, m) =>
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
