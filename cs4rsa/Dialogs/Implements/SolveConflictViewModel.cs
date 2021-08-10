using cs4rsa.BaseClasses;
using cs4rsa.Dialogs.DialogResults;
using cs4rsa.Messages;
using cs4rsa.Models;
using LightMessageBus;
using System;

namespace cs4rsa.Dialogs.Implements
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
            CloseDialogCommand = new RelayCommand(OnCloseDialog);
        }

        private void OnCloseDialog(object obj)
        {
            CloseDialogCallback.Invoke(null);
        }

        private void OnRemoveClassGroup2(object obj)
        {
            MessageBus.Default.Publish<RemoveAChoiceClassGroupMessage>(new RemoveAChoiceClassGroupMessage(_classGroup2Name));
            SolveConflictResult result = new SolveConflictResult(_classGroup2Name);
            CloseDialogCallback.Invoke(result);
        }

        private void OnRemoveClassGroup1(object obj)
        {
            MessageBus.Default.Publish<RemoveAChoiceClassGroupMessage>(new RemoveAChoiceClassGroupMessage(_classGroup1Name));
            SolveConflictResult result = new SolveConflictResult(_classGroup1Name);
            CloseDialogCallback.Invoke(result);
        }
    }
}
