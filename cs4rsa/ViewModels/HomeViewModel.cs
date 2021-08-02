using cs4rsa.Crawler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.BaseClasses;
using cs4rsa.Messages;
using LightMessageBus.Interfaces;
using LightMessageBus;

namespace cs4rsa.ViewModels
{
    public class HomeViewModel: NotifyPropertyChangedBase
    {
        private HomeCourseSearch _homeCourseSearch = HomeCourseSearch.GetInstance();
        private string _currentYearValue;
        public string CurrentYearValue
        {
            get { return _currentYearValue; }
            set { _currentYearValue = value; RaisePropertyChanged(); }
        }

        private string _currentSemesterValue;
        public string CurrentSemesterValue
        {
            get { return _currentSemesterValue; }
            set { _currentSemesterValue = value; RaisePropertyChanged(); }
        }

        private string _currentYearInfo;
        public string CurrentYearInfo
        {
            get { return _currentYearInfo; }
            set { _currentYearInfo = value; RaisePropertyChanged(); }
        }

        private string _currentSemesterInfo;
        public string CurrentSemesterInfo
        {
            get { return _currentSemesterInfo; }
            set { _currentSemesterInfo = value; RaisePropertyChanged(); }
        }

        public RelayCommand UpdateSubjectDatabaseCommand { get; set; }

        public HomeViewModel()
        {
            _currentYearValue = _homeCourseSearch.CurrentYearValue;
            _currentSemesterValue = _homeCourseSearch.CurrentSemesterValue;
            _currentSemesterInfo = _homeCourseSearch.CurrentSemesterInfo;
            _currentYearInfo = _homeCourseSearch.CurrentYearInfo;
            UpdateSubjectDatabaseCommand = new RelayCommand(OnUpdate);
        }

        private void OnUpdate(object obj)
        {
            MessageBus.Default.Publish(new UpdateSubjectDatabase(null));
        }
    }
}
