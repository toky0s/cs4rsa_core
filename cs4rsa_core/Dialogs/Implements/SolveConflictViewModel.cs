using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.BaseClasses;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Messages.Publishers.Dialogs;
using Cs4rsa.Services.ConflictSvc.Interfaces;
using Cs4rsa.Services.SubjectCrawlerSvc.Models;

namespace Cs4rsa.Dialogs.Implements
{
    public partial class SolveConflictViewModel : DialogVmBase
    {
        [ObservableProperty] private SchoolClassModel _firstSc;
        [ObservableProperty] private SchoolClassModel _secondSc;
        [ObservableProperty] private string _fColor;
        [ObservableProperty] private string _sColor;

        public RelayCommand RemoveCgFirstCmd { get; set; }
        public RelayCommand RemoveCgSecondCmd { get; set; }

        public SolveConflictViewModel(
            IConflictModel conflictModel,
            IUnitOfWork unitOfWork
        )
        {
            FirstSc = conflictModel.FirstSchoolClass;
            SecondSc = conflictModel.SecondSchoolClass;
            RemoveCgFirstCmd = new(() => Messenger.Send(new SolveConflictVmMsgs.RemoveChoicedClassMsg(_firstSc.SchoolClass.ClassGroupName)));
            RemoveCgSecondCmd = new(() => Messenger.Send(new SolveConflictVmMsgs.RemoveChoicedClassMsg(_secondSc.SchoolClass.ClassGroupName)));
            FColor = unitOfWork.Keywords.GetColorWithSubjectCode(_firstSc.SubjectCode);
            SColor = unitOfWork.Keywords.GetColorWithSubjectCode(_secondSc.SubjectCode);
        }
    }
}
