using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.BaseClasses;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Messages.Publishers.Dialogs;
using Cs4rsa.Services.ConflictSvc.Interfaces;
using Cs4rsa.Services.ConflictSvc.Models;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes;
using Cs4rsa.Services.SubjectCrawlerSvc.Models;

namespace Cs4rsa.Dialogs.Implements
{
    public partial class SolveConflictViewModel : DialogVmBase
    {
        [ObservableProperty]
        private SchoolClassModel _firstSC;
        [ObservableProperty]
        private SchoolClassModel _secondSC;

        public RelayCommand RemoveCgFirstCmd { get; set; }
        public RelayCommand RemoveCgSecondCmd { get; set; }

        public SolveConflictViewModel(
            IConflictModel conflictModel,
            IUnitOfWork unitOfWork
        )
        {
            _firstSC = conflictModel.FirstSchoolClass;
            _secondSC = conflictModel.SecondSchoolClass;
            RemoveCgFirstCmd = new(() => Messenger.Send(new SolveConflictVmMsgs.RemoveChoicedClassMsg(_firstSC.SchoolClass.ClassGroupName)));
            RemoveCgSecondCmd = new(() => Messenger.Send(new SolveConflictVmMsgs.RemoveChoicedClassMsg(_secondSC.SchoolClass.ClassGroupName)));
            FColor = unitOfWork.Keywords.GetColorWithSubjectCode(_firstSC.SubjectCode);
            SColor = unitOfWork.Keywords.GetColorWithSubjectCode(_secondSC.SubjectCode);
        }
    }
}
