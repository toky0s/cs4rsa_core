using ConflictService.Models;

using cs4rsa_core.BaseClasses;
using cs4rsa_core.Dialogs.DialogResults;
using cs4rsa_core.Messages;

using LightMessageBus;

using Microsoft.Toolkit.Mvvm.Input;

using System;

namespace cs4rsa_core.Dialogs.Implements
{
    public class SolveConflictViewModel : ViewModelBase
    {
        private string _classGroup1Name;
        public string ClassGroup1Name
        {
            get { return _classGroup1Name; }
            set { _classGroup1Name = value; OnPropertyChanged(); }
        }
        private string _classGroup2Name;
        public string ClassGroup2Name
        {
            get { return _classGroup2Name; }
            set { _classGroup2Name = value; OnPropertyChanged(); }
        }

        private ConflictModel _conflictModel;

        public ConflictModel ConflictModel
        {
            get { return _conflictModel; }
            set
            {
                _conflictModel = value;
            }
        }

        public RelayCommand RemoveClassGroup1Command { get; set; }
        public RelayCommand RemoveClassGroup2Command { get; set; }
        public RelayCommand CloseDialogCommand { get; set; }

        public Action<SolveConflictResult> CloseDialogCallback { get; set; }

        public SolveConflictViewModel(ConflictModel conflictModel)
        {
            _classGroup1Name = conflictModel.FirstSchoolClass.ClassGroupName;
            _classGroup2Name = conflictModel.SecondSchoolClass.ClassGroupName;
            RemoveClassGroup1Command = new RelayCommand(OnRemoveClassGroup1);
            RemoveClassGroup2Command = new RelayCommand(OnRemoveClassGroup2);
        }

        /// <summary>
        /// Loại bỏ Class2
        /// </summary>
        private void OnRemoveClassGroup2()
        {
            MessageBus.Default.Publish(new RemoveChoicedClassMessage(_classGroup2Name));
            SolveConflictResult result = new(_classGroup2Name);
            CloseDialogCallback.Invoke(result);
        }

        /// <summary>
        /// Loại bỏ Class1
        /// </summary>
        private void OnRemoveClassGroup1()
        {
            MessageBus.Default.Publish(new RemoveChoicedClassMessage(_classGroup1Name));
            SolveConflictResult result = new(_classGroup1Name);
            CloseDialogCallback.Invoke(result);
        }
    }
}
