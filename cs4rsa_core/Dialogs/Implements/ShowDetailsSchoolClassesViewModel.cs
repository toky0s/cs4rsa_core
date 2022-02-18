using cs4rsa_core.Dialogs.DialogResults;
using cs4rsa_core.Models;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;

namespace cs4rsa_core.Dialogs.Implements
{
    public class ShowDetailsSchoolClassesViewModel
    {
        public ClassGroupModel ClassGroupModel { get; set; }
        public SchoolClassModel SelectedSchoolClassModel { get; set; }
        public ObservableCollection<SchoolClassModel> SchoolClassModels { get; set; }
        public Action<ClassGroupResult> CloseDialogCallback { get; set; }
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
                CloseDialogCallback(classGroupResult);
            }
        }
    }
}
