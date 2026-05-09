using Cs4rsa.Database.Interfaces;
using Cs4rsa.Module.ManuallySchedule.Models;
using Cs4rsa.Service.Conflict.Interfaces;
using Cs4rsa.Service.Conflict.Models;

using Microsoft.Extensions.Logging;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

using System;

namespace Cs4rsa.Module.ManuallySchedule.Dialogs.ViewModels
{
    public partial class SolveConflictViewModel : BindableBase, IDialogAware
    {
        private ClassGroupModel _classGroupModel_A;
        public ClassGroupModel ClassGroupModel_A
        {
            get { return _classGroupModel_A; }
            set { SetProperty(ref _classGroupModel_A, value); }
        }

        private ClassGroupModel _classGroupModel_B;
        public ClassGroupModel ClassGroupModel_B
        {
            get { return _classGroupModel_B; }
            set { SetProperty(ref _classGroupModel_B, value); }
        }

        private string _fColor;
        public string FColor
        {
            get { return _fColor; }
            set { SetProperty(ref _fColor, value); }
        }


        public event Action<IDialogResult> RequestClose;


        private void CloseDialogWithRemovedLesson(ClassGroupModel classGroupModel)
        {
            var parameter = new DialogParameters
            {
                { "RemovedClassGroupModel", classGroupModel }
            };
            RequestClose.Invoke(new DialogResult(ButtonResult.OK, parameter));
        }

        private DelegateCommand _removeCgFirstCmd;
        public DelegateCommand RemoveCgFirstCmd =>
            _removeCgFirstCmd ?? (_removeCgFirstCmd = new DelegateCommand(ExecuteRemoveCgFirstCmd, CanExecuteRemoveCgFirstCmd));

        void ExecuteRemoveCgFirstCmd()
        {
            //_eventAggregator.GetEvent<SolveConflictVmMsgs.RemoveChoicedClassMsg>().Publish(_lessonA.ClassGroupName);
            CloseDialogWithRemovedLesson(_classGroupModel_A);
        }

        bool CanExecuteRemoveCgFirstCmd()
        {
            return true;
        }

        private DelegateCommand _removeCgSecondCmd;
        private readonly ILogger<SolveConflictViewModel> _logger;

        public DelegateCommand RemoveCgSecondCmd =>
            _removeCgSecondCmd ?? (_removeCgSecondCmd = new DelegateCommand(ExecuteRemoveCgSecondCmd, CanExecuteRemoveCgSecondCmd));

        void ExecuteRemoveCgSecondCmd()
        {
            //_eventAggregator.GetEvent<SolveConflictVmMsgs.RemoveChoicedClassMsg>().Publish(_lessonB.ClassGroupName);
            CloseDialogWithRemovedLesson(_classGroupModel_B);
        }

        bool CanExecuteRemoveCgSecondCmd()
        {
            return true;
        }


        public string Title => "Solve Conflicts";


        public SolveConflictViewModel(ILogger<SolveConflictViewModel> logger)
        {
            _logger = logger;
        }
        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            _logger.LogTrace("SolveConflict dialog closed");
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            var conflictModel = parameters.GetValue<IConflictModel[]>("ConflictModels");
            ClassGroupModel_A = parameters.GetValue<ClassGroupModel>("ClassGroupModelA");
            ClassGroupModel_B = parameters.GetValue<ClassGroupModel>("ClassGroupModelB");
        }
    }
}
