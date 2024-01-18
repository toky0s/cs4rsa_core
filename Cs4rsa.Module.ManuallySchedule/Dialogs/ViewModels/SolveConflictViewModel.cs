using Cs4rsa.Database.Interfaces;
using Cs4rsa.Module.ManuallySchedule.Events;
using Cs4rsa.Service.Conflict.Interfaces;
using Cs4rsa.Service.Conflict.Models;

using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace Cs4rsa.Module.ManuallySchedule.Dialogs.ViewModels
{
    public partial class SolveConflictViewModel : BindableBase
    {
        private Lesson _lessonA;
        public Lesson LessonA
        {
            get { return _lessonA; }
            set { SetProperty(ref _lessonA, value); }
        }

        private Lesson _lessonB;
        public Lesson LessonB
        {
            get { return _lessonB; }
            set { SetProperty(ref _lessonB, value); }
        }

        private string _fColor;
        public string FColor
        {
            get { return _fColor; }
            set { SetProperty(ref _fColor, value); }
        }

        private string _sColor;
        public string SColor
        {
            get { return _sColor; }
            set { SetProperty(ref _sColor, value); }
        }

        public DelegateCommand RemoveCgFirstCmd { get; set; }
        public DelegateCommand RemoveCgSecondCmd { get; set; }

        public SolveConflictViewModel(
            IConflictModel conflictModel,
            IUnitOfWork unitOfWork,
            IEventAggregator eventAggregator
        )
        {
            LessonA = conflictModel.LessonA;
            LessonB = conflictModel.LessonB;
            RemoveCgFirstCmd = new DelegateCommand(() => eventAggregator.GetEvent<SolveConflictVmMsgs.RemoveChoicedClassMsg>().Publish(_lessonA.ClassGroupName));
            RemoveCgSecondCmd = new DelegateCommand(() => eventAggregator.GetEvent<SolveConflictVmMsgs.RemoveChoicedClassMsg>().Publish(_lessonB.ClassGroupName));
            FColor = unitOfWork.Keywords.GetColorWithSubjectCode(_lessonA.SubjectCode);
            SColor = unitOfWork.Keywords.GetColorWithSubjectCode(_lessonB.SubjectCode);
        }
    }
}
