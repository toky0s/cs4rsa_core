using Cs4rsa.Module.ManuallySchedule.Models;

using Prism.Mvvm;

namespace Cs4rsa.Module.ManuallySchedule.Dialogs.ViewModels
{
    public class ShowDetailsSubjectViewModel : BindableBase
    {
        private SubjectModel _subjectModel;
        public SubjectModel SubjectModel
        {
            get { return _subjectModel; }
            set { SetProperty(ref _subjectModel, value); }
        }

        public ShowDetailsSubjectViewModel()
        {

        }
    }
}
