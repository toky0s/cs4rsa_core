using cs4rsa_core.BaseClasses;

namespace cs4rsa_core.Dialogs.Implements
{
    class SubjectDownloadingViewModel : ViewModelBase
    {
        private string _subjectName;

        public string SubjectName
        {
            get { return _subjectName; }
            set { _subjectName = value; OnPropertyChanged(); }
        }

        private string _subjectCode;

        public string SubjectCode
        {
            get { return _subjectCode; }
            set { _subjectCode = value; OnPropertyChanged(); }
        }
    }
}
