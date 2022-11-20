using Cs4rsa.BaseClasses;
using Cs4rsa.Services.SubjectCrawlerSvc.Models;

namespace Cs4rsa.Dialogs.Implements
{
    public class ShowDetailsSubjectViewModel : ViewModelBase
    {
        private SubjectModel _subjectModel;

        public SubjectModel SubjectModel
        {
            get { return _subjectModel; }
            set { _subjectModel = value; OnPropertyChanged(); }
        }
        public ShowDetailsSubjectViewModel()
        {

        }
    }
}
