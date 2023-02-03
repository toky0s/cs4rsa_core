using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.BaseClasses;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Messages.Publishers.Dialogs;
using Cs4rsa.Services.ConflictSvc.Models;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes;

namespace Cs4rsa.Dialogs.Implements
{
    internal partial class SolveConflictViewModel : DialogVmBase
    {
        [ObservableProperty]
        private SchoolClass _firstSC;
        [ObservableProperty]
        private SchoolClass _secondSC;
        [ObservableProperty]
        private string fColor;
        [ObservableProperty]
        private string sColor;

        public RelayCommand RemoveCgFirstCmd { get; set; }
        public RelayCommand RemoveCgSecondCmd { get; set; }

        public SolveConflictViewModel(
            ConflictModel conflictModel,
            IUnitOfWork unitOfWork
        )
        {
            _firstSC = conflictModel.FirstSchoolClass;
            _secondSC = conflictModel.SecondSchoolClass;
            RemoveCgFirstCmd = new(() => Messenger.Send(new SolveConflictVmMsgs.RemoveChoicedClassMsg(_firstSC.ClassGroupName)));
            RemoveCgSecondCmd = new(() => Messenger.Send(new SolveConflictVmMsgs.RemoveChoicedClassMsg(_secondSC.ClassGroupName)));
            FColor = unitOfWork.Keywords.GetColorWithSubjectCode(_firstSC.SubjectCode);
            SColor = unitOfWork.Keywords.GetColorWithSubjectCode(_secondSC.SubjectCode);
        }
    }
}
