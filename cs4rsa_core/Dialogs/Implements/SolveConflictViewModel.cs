using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.BaseClasses;
using Cs4rsa.Dialogs.DialogResults;
using Cs4rsa.Messages.Publishers.Dialogs;
using Cs4rsa.Services.ConflictSvc.Models;

using System;

namespace Cs4rsa.Dialogs.Implements
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
            Messenger.Send(new SolveConflictVmMsgs.RemoveChoicedClassMsg(_classGroup2Name));
            SolveConflictResult result = new(_classGroup2Name);
            CloseDialogCallback.Invoke(result);
        }

        /// <summary>
        /// Loại bỏ SessionInputVmMsgs
        /// </summary>
        private void OnRemoveClassGroup1()
        {
            Messenger.Send(new SolveConflictVmMsgs.RemoveChoicedClassMsg(_classGroup1Name));
            SolveConflictResult result = new(_classGroup1Name);
            CloseDialogCallback.Invoke(result);
        }
    }
}
