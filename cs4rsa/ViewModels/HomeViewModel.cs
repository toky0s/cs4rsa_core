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
using cs4rsa.Settings;

namespace cs4rsa.ViewModels
{
    public class HomeViewModel: ViewModelBase, IMessageHandler<UpdateSuccessMessage>
    {
        private HomeCourseSearch _homeCourseSearch = HomeCourseSearch.GetInstance();
        private string _currentYearValue;
        public string CurrentYearValue
        {
            get { return _currentYearValue; }
            set { _currentYearValue = value; OnPropertyChanged(); }
        }

        private string _currentSemesterValue;
        public string CurrentSemesterValue
        {
            get { return _currentSemesterValue; }
            set { _currentSemesterValue = value; OnPropertyChanged(); }
        }

        private string _currentYearInfo;
        public string CurrentYearInfo
        {
            get { return _currentYearInfo; }
            set { _currentYearInfo = value; OnPropertyChanged(); }
        }

        private string _currentSemesterInfo;
        public string CurrentSemesterInfo
        {
            get { return _currentSemesterInfo; }
            set { _currentSemesterInfo = value; OnPropertyChanged(); }
        }

        private bool _isNewSemester;

        public bool IsNewSemester
        {
            get { return _isNewSemester; }
            set { _isNewSemester = value; OnPropertyChanged(); }
        }


        public RelayCommand UpdateSubjectDatabaseCommand { get; set; }

        public HomeViewModel()
        {
            MessageBus.Default.FromAny().Where<UpdateSuccessMessage>().Notify(this);
            _currentYearValue = _homeCourseSearch.CurrentYearValue;
            _currentSemesterValue = _homeCourseSearch.CurrentSemesterValue;
            _currentSemesterInfo = _homeCourseSearch.CurrentSemesterInfo;
            _currentYearInfo = _homeCourseSearch.CurrentYearInfo;
            UpdateSubjectDatabaseCommand = new RelayCommand(OnUpdate);

            LoadIsNewSemester();
        }

        private void OnUpdate(object obj)
        {
            MessageBus.Default.Publish(new UpdateSubjectDatabase(null));
        }

        public void LoadIsNewSemester()
        {
            SettingsReader settingReader = new SettingsReader();
            Cs4rsaSetting setting = settingReader.Setting;
            if (setting.CurrentSemesterValue != _currentSemesterValue || setting.CurrentYearValue != _currentYearValue)
                IsNewSemester = true;
            else
                IsNewSemester = false;
        }

        public void Handle(UpdateSuccessMessage message)
        {
            LoadIsNewSemester();
        }
    }
}
