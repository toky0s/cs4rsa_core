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
        private ClassGroupModel _classGroup1;
        public ClassGroupModel ClassGroup1
        {
            get { return _classGroup1; }
            set { _classGroup1 = value; OnPropertyChanged(); }
        }
        private ClassGroupModel _classGroup2;
        public ClassGroupModel ClassGroup2
        {
            get { return _classGroup2; }
            set { _classGroup2 = value; OnPropertyChanged(); }
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
            _classGroup1 = conflictModel.FirstClassGroupModel;
            _classGroup2 = conflictModel.SecondClassGroupModel;
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
            MessageBus.Default.Publish<RemoveAChoiceClassGroupMessage>(new RemoveAChoiceClassGroupMessage(_classGroup2));
            SolveConflictResult result = new SolveConflictResult(_classGroup2);
            CloseDialogCallback.Invoke(result);
        }

        private void OnRemoveClassGroup1(object obj)
        {
            MessageBus.Default.Publish<RemoveAChoiceClassGroupMessage>(new RemoveAChoiceClassGroupMessage(_classGroup1));
            SolveConflictResult result = new SolveConflictResult(_classGroup1);
            CloseDialogCallback.Invoke(result);
        }
    }
}
