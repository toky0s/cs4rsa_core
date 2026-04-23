using Cs4rsa.Module.ManuallySchedule.Models;
using Cs4rsa.Service.Dialog;

namespace Cs4rsa.Module.ManuallySchedule.Dialogs.ViewModels
{
    public class ShowDetailsSubjectUCViewModel : DialogViewModelBase
    {
        private SubjectModel _subjectModel;
        public SubjectModel SubjectModel
        {
            get { return _subjectModel; }
            set { SetProperty(ref _subjectModel, value); }
        }

        private string _url;

        public string Url
        {
            get { return _url; }
            set { SetProperty(ref _url, value); }
        }

        public ShowDetailsSubjectUCViewModel() : base("Show Subject Details")
        {

        }
    }
}
