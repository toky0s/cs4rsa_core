using Cs4rsa.Common.Interfaces;
using Cs4rsa.Module.ManuallySchedule.Dialogs.Models;
using Cs4rsa.Module.ManuallySchedule.Models;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Cs4rsa.Module.ManuallySchedule.Dialogs.ViewModels
{
    public class DetailsSchoolClassesViewModel : BindableBase, IDialogAware
    {
        private ClassGroupModel _classGroupModel;
        public ClassGroupModel ClassGroupModel
        {
            get => _classGroupModel;
            set { SetProperty(ref _classGroupModel, value); }
        }

        private SchoolClassModel _selectedSchoolClassModel;
        public SchoolClassModel SelectedSchoolClassModel
        {
            get => _selectedSchoolClassModel;
            set { SetProperty(ref _selectedSchoolClassModel, value); }
        }
        
        public ObservableCollection<SchoolClassModel> _schoolClassModels;
        public ObservableCollection<SchoolClassModel> SchoolClassModels => _schoolClassModels ?? (_schoolClassModels = new ObservableCollection<SchoolClassModel>());
        
        public DelegateCommand _pickCommand;
        public DelegateCommand PickCommand => _pickCommand ?? (_pickCommand = new DelegateCommand(ExecutePickCommand, () => SelectedSchoolClassModel != null)
            .ObservesProperty(() => SelectedSchoolClassModel));

        private DelegateCommand _openLinkCommand;
        public DelegateCommand OpenLinkCommand =>
            _openLinkCommand ?? (_openLinkCommand = new DelegateCommand(ExecuteOpenLinkCommand, CanExecuteOpenLinkCommand));

        void ExecuteOpenLinkCommand()
        {
            var link = ClassGroupModel.ClassGroup.Subject.GetLink();
            _openInBrowser.Open(link);
        }

        bool CanExecuteOpenLinkCommand()
        {
            return true;
        }

        public string Title => "Select Class";

        public event Action<IDialogResult> RequestClose;

        private readonly IOpenInBrowser _openInBrowser;
        public DetailsSchoolClassesViewModel(IOpenInBrowser openInBrowser)
        {
            _openInBrowser = openInBrowser;
        }

        public void ExecutePickCommand()
        {
            var classGroupResult = new ClassGroupResult()
            {
                ClassGroupModel = ClassGroupModel,
                SelectedSchoolClassModel = SelectedSchoolClassModel
            };
            //_eventAggregator.GetEvent<ShowDetailsSchoolClassesVmMsgs.ExitChooseMsg>().Publish(classGroupResult);
            RequestClose?.Invoke(new DialogResult(ButtonResult.OK, new DialogParameters()
            {
                { "ClassGroupModel", ClassGroupModel},
                { "SelectedSchoolClassModel", SelectedSchoolClassModel}
            }));
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            return;
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            ClassGroupModel = parameters.GetValue<ClassGroupModel>("SelectedClassGroup");
            var schoolClasses = parameters.GetValue<ImmutableArray<SchoolClassModel>>("SchoolClassModels");
            SchoolClassModels.AddRange(schoolClasses);
        }
    }
}
