using Cs4rsa.Module.ManuallySchedule.Dialogs.Models;
using Cs4rsa.Module.ManuallySchedule.Events;
using Cs4rsa.Module.ManuallySchedule.Models;
using Cs4rsa.Service.Dialog.Interfaces;

using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

using System.Collections.ObjectModel;

namespace Cs4rsa.Module.ManuallySchedule.Dialogs.ViewModels
{
    public class ShowDetailsSchoolClassesViewModel : BindableBase
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

        public ObservableCollection<SchoolClassModel> SchoolClassModels { get; set; }
        public DelegateCommand PickCommand { get; set; }

        private readonly IEventAggregator _eventAggregator;
        private readonly IDialogService _dialogService;

        public ShowDetailsSchoolClassesViewModel(IEventAggregator eventAggregator, IDialogService dialogService)
        {
            _eventAggregator = eventAggregator;
            _dialogService = dialogService;

            SchoolClassModels = new ObservableCollection<SchoolClassModel>();
            PickCommand = new DelegateCommand(OnPick);
        }

        public void OnPick()
        {
            if (SelectedSchoolClassModel != null)
            {
                var classGroupResult = new ClassGroupResult()
                {
                    ClassGroupModel = ClassGroupModel,
                    SelectedSchoolClassModel = SelectedSchoolClassModel
                };
                _eventAggregator.GetEvent<ShowDetailsSchoolClassesVmMsgs.ExitChooseMsg>().Publish(classGroupResult);
                _dialogService.CloseDialog();
            }
        }
    }
}
