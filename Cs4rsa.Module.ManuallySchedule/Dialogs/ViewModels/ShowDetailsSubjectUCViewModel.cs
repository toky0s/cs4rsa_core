using Cs4rsa.Module.ManuallySchedule.Models;
using Cs4rsa.Service.Dialog;

using Prism.Mvvm;
using Prism.Services.Dialogs;

using System;

namespace Cs4rsa.Module.ManuallySchedule.Dialogs.ViewModels
{
    public class ShowDetailsSubjectUCViewModel : BindableBase, IDialogAware
    {
        private SubjectModel _subjectModel;
        public SubjectModel SubjectModel
        {
            get { return _subjectModel; }
            set { SetProperty(ref _subjectModel, value); }
        }

        public event Action<IDialogResult> RequestClose;

        private Uri _url;
        public Uri Url
        {
            get { return _url; }
            set { SetProperty(ref _url, value); }
        }


        public string Title => "Show Subject Details";

        public ShowDetailsSubjectUCViewModel()
        {

        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            var urlString = parameters.GetValue<string>("Url");
            Url = Uri.TryCreate(urlString, UriKind.Absolute, out var uri) ? uri : null;
            SubjectModel = parameters.GetValue<SubjectModel>("SubjectModel");
        }
    }
}
