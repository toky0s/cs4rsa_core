using cs4rsa_core.Dialogs.DialogResults;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using cs4rsa_core.BaseClasses;
using CommunityToolkit.Mvvm.Messaging;
using cs4rsa_core.Services.SubjectCrawlerSvc.Models;
using cs4rsa_core.Messages.Publishers.Dialogs;

namespace cs4rsa_core.Dialogs.Implements
{
    public class ShowDetailsSchoolClassesViewModel: ViewModelBase
    {
        private ClassGroupModel _classGroupModel;
        public ClassGroupModel ClassGroupModel
        {
            get => _classGroupModel;
            set
            {
                _classGroupModel = value;
                OnPropertyChanged();
            }
        }

        private SchoolClassModel _selectedSchoolClassModel;
        public SchoolClassModel SelectedSchoolClassModel 
        {   
            get => _selectedSchoolClassModel; 
            set
            {
                _selectedSchoolClassModel = value; 
                OnPropertyChanged(); 
            } 
        }
        public ObservableCollection<SchoolClassModel> SchoolClassModels { get; set; }
        public RelayCommand PickCommand { get; set; }

        public ShowDetailsSchoolClassesViewModel()
        {
            SchoolClassModels = new();
            PickCommand = new(OnPick);
        }

        public void OnPick()
        {
            if (SelectedSchoolClassModel != null)
            {
                ClassGroupResult classGroupResult = new()
                {
                    ClassGroupModel = ClassGroupModel,
                    SelectedSchoolClassModel = SelectedSchoolClassModel
                };
                Messenger.Send(new ShowDetailsSchoolClassesVmMsgs.ExitChooseMsg(classGroupResult));
                CloseDialog();
            }
        }
    }
}
