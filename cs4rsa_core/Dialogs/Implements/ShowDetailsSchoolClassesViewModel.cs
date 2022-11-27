using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.BaseClasses;
using Cs4rsa.Dialogs.DialogResults;
using Cs4rsa.Messages.Publishers.Dialogs;
using Cs4rsa.Services.SubjectCrawlerSvc.Models;

using System.Collections.ObjectModel;

namespace Cs4rsa.Dialogs.Implements
{
    public class ShowDetailsSchoolClassesViewModel : ViewModelBase
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
