using Cs4rsa.Database.Interfaces;
using Cs4rsa.Module.ManuallySchedule.Models;
using Cs4rsa.Module.ManuallySchedule.Utils;
using Cs4rsa.Service.Conflict.DataTypes;
using Cs4rsa.Service.Conflict.DataTypes.Enums;
using Cs4rsa.Service.Conflict.Interfaces;
using Cs4rsa.Service.Conflict.Models;
using Cs4rsa.Service.SubjectCrawler.Utils;
using Cs4rsa.Module.Shared;
using Cs4rsa.UI.ScheduleTable.Models;

using Microsoft.Extensions.Logging;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Documents;

namespace Cs4rsa.Module.ManuallySchedule.Dialogs.ViewModels
{
    public class ConflictInfo
    {
        public string Day { get; set; }
        public string ClassGroupModel_A_TimeRange { get; set; }
        public string ClassGroupModel_B_TimeRange { get; set; }
        public string ConflictDurian { get; set; }
    }

    public partial class SolveConflictViewModel : BindableBase, IDialogAware
    {
        private ObservableCollection<ConflictInfo> _conflictInfos;
        public ObservableCollection<ConflictInfo> ConflictInfos
        {
            get { return _conflictInfos; }
            set { SetProperty(ref _conflictInfos, value); }
        }
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
            ConflictInfos = new ObservableCollection<ConflictInfo>();
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
            var conflictModel = parameters.GetValue<ConflictModel>("ConflictModel");
            ClassGroupModel_A = parameters.GetValue<ClassGroupModel>("ClassGroupModelA");
            ClassGroupModel_B = parameters.GetValue<ClassGroupModel>("ClassGroupModelB");

            ClassGroupModel_A.CurrentSchoolClassModels.ForEach(item =>
            {
                foreach (var u in item.SchoolClass.SchoolClassUnits)
                {
                    _logger.LogInformation("ClassGroupModel_A Start: {Start}, End: {End}, Day: {day}",
                        u.Start, u.End, u.DayOfWeek.ToCs4rsaVietnamese());
                }
            });

            ClassGroupModel_B.CurrentSchoolClassModels.ForEach(item =>
            {
                foreach (var u in item.SchoolClass.SchoolClassUnits)
                {
                    _logger.LogInformation("ClassGroupModel_B Start: {Start}, End: {End}, Day: {day}",
                        u.Start, u.End, u.DayOfWeek.ToCs4rsaVietnamese());
                }
            });


            foreach (KeyValuePair<DayOfWeek, IEnumerable<StudyTimeIntersect>> item in conflictModel.ConflictTime.ConflictTimes)
            {
                var day = item.Key;
                foreach (var intersect in item.Value)
                {
                    var unitA = ClassGroupModel_A.CurrentSchoolClassModels
                        .SelectMany(schClassModel => schClassModel.SchoolClass.SchoolClassUnits)
                        .Where(unit => unit.DayOfWeek == day 
                            && Shared.Utils.IsOverlap(unit.Start, unit.End, intersect.Start, intersect.End))
                        .First();

                    var unitB = ClassGroupModel_B.CurrentSchoolClassModels
                        .SelectMany(schClassModel => schClassModel.SchoolClass.SchoolClassUnits)
                        .Where(unit => unit.DayOfWeek == day
                            && Shared.Utils.IsOverlap(unit.Start, unit.End, intersect.Start, intersect.End))
                        .First();

                    ConflictInfos.Add(new ConflictInfo
                    {
                        Day = day.ToCs4rsaVietnamese(),
                        ClassGroupModel_A_TimeRange = $"{unitA.Start:HH:mm} - {unitA.End:HH:mm}",
                        ClassGroupModel_B_TimeRange = $"{unitB.Start:HH:mm} - {unitB.End:HH:mm}",
                        ConflictDurian = $"{intersect.StartString} - {intersect.EndString}"
                    });
                }      
            }           
        }
    }
}
