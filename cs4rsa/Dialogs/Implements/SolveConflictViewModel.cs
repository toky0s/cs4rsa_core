using cs4rsa.Dialogs.DialogResults;
using cs4rsa.Dialogs.DialogService;
using cs4rsa.Messages;
using cs4rsa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightMessageBus;

namespace cs4rsa.Dialogs.Implements
{
    public class SolveConflictViewModel: DialogViewModelBase<SolveConflictResult>
    {
        private ClassGroupModel _classGroup1;
        public ClassGroupModel ClassGroup1
        {
            get { return _classGroup1; }
            set { _classGroup1 = value; RaisePropertyChanged(); }
        }
        private ClassGroupModel _classGroup2;
        public ClassGroupModel ClassGroup2
        {
            get { return _classGroup2; }
            set { _classGroup2 = value; RaisePropertyChanged(); }
        }

        public RelayCommand RemoveClassGroup1Command { get; set; }
        public RelayCommand RemoveClassGroup2Command { get; set; }

        public SolveConflictViewModel(ConflictModel conflictModel)
        {
            _classGroup1 = conflictModel.FirstClassGroupModel;
            _classGroup2 = conflictModel.SecondClassGroupModel;
            RemoveClassGroup1Command = new RelayCommand(OnRemoveClassGroup1);
            RemoveClassGroup2Command = new RelayCommand(OnRemoveClassGroup2);
        }

        private void OnRemoveClassGroup2(object obj)
        {
            MessageBus.Default.Publish<RemoveAChoiceClassGroupMessage>(new RemoveAChoiceClassGroupMessage(_classGroup2));
            SolveConflictResult result = new SolveConflictResult(_classGroup2);
            CloseDialogWithResult(result);
        }

        private void OnRemoveClassGroup1(object obj)
        {
            MessageBus.Default.Publish<RemoveAChoiceClassGroupMessage>(new RemoveAChoiceClassGroupMessage(_classGroup1));
            SolveConflictResult result = new SolveConflictResult(_classGroup1);
            CloseDialogWithResult(result);
        }
    }
}
