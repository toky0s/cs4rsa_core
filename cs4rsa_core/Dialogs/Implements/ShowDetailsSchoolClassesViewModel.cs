using cs4rsa_core.Dialogs.DialogResults;
using CommunityToolkit.Mvvm.Input;
using SubjectCrawlService1.Models;
using System.Collections.ObjectModel;
using cs4rsa_core.BaseClasses;
using CommunityToolkit.Mvvm.Messaging;
using static cs4rsa_core.Messages.Publishers.Dialogs.ShowDetailsSchoolClassesVmMsgs;

namespace cs4rsa_core.Dialogs.Implements
{
    public class ShowDetailsSchoolClassesViewModel: ViewModelBase
    {
        public ClassGroupModel ClassGroupModel { get; set; }
        public SchoolClassModel SelectedSchoolClassModel { get; set; }
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
                    SelectedRegisterCode = SelectedSchoolClassModel.RegisterCode
                };
                Messenger.Send(new ExitChooseMsg(classGroupResult));
                CloseDialog();
            }
        }
    }
}
