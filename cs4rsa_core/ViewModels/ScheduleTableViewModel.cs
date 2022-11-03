using cs4rsa_core.BaseClasses;
using cs4rsa_core.Messages.Publishers;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using cs4rsa_core.Services.SubjectCrawlerSvc.Models;
using cs4rsa_core.Services.SubjectCrawlerSvc.DataTypes;
using cs4rsa_core.Services.SubjectCrawlerSvc.DataTypes.Enums;
using cs4rsa_core.Services.ConflictSvc.Models;
using System.Windows;
using System.Threading.Tasks;
using cs4rsa_core.Commons.Models;
using cs4rsa_core.Commons.Enums;
using System;
using cs4rsa_core.Controls;

namespace cs4rsa_core.ViewModels
{
    class ScheduleTableViewModel : ViewModelBase
    {
        private IEnumerable<ClassGroupModel> _classGroupModels;
        private IEnumerable<ConflictModel> _conflictModels;
        private IEnumerable<PlaceConflictFinderModel> _placeConflictFinderModels;

        public ObservableCollection<TimeBlock> Phase1_Monday { get; set; }
        public ObservableCollection<TimeBlock> Phase1_Tuesday { get; set; }
        public ObservableCollection<TimeBlock> Phase1_Wednesday { get; set; }
        public ObservableCollection<TimeBlock> Phase1_Thursday { get; set; }
        public ObservableCollection<TimeBlock> Phase1_Friday { get; set; }
        public ObservableCollection<TimeBlock> Phase1_Saturday { get; set; }
        public ObservableCollection<TimeBlock> Phase1_Sunday { get; set; }

        public ObservableCollection<TimeBlock> Phase2_Monday { get; set; }
        public ObservableCollection<TimeBlock> Phase2_Tuesday { get; set; }
        public ObservableCollection<TimeBlock> Phase2_Wednesday { get; set; }
        public ObservableCollection<TimeBlock> Phase2_Thursday { get; set; }
        public ObservableCollection<TimeBlock> Phase2_Friday { get; set; }
        public ObservableCollection<TimeBlock> Phase2_Saturday { get; set; }
        public ObservableCollection<TimeBlock> Phase2_Sunday { get; set; }

        public ObservableCollection<string> Timelines { get; set; }

        private readonly ObservableCollection<TimeBlock>[] week1;
        private readonly ObservableCollection<TimeBlock>[] week2;

        public ScheduleTableViewModel()
        {
            WeakReferenceMessenger.Default.Register<ChoicedSessionVmMsgs.ChoiceChangedMsg>(this, (r, m) =>
            {
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    _classGroupModels = m.Value;
                    ReloadSchedule();
                    return Task.CompletedTask;
                });
            });

            WeakReferenceMessenger.Default.Register<ChoicedSessionVmMsgs.ConflictCollChangedMsg>(this, (r, m) =>
            {
                _conflictModels = m.Value;
                ReloadSchedule();
            });

            WeakReferenceMessenger.Default.Register<ChoicedSessionVmMsgs.PlaceConflictCollChangedMsg>(this, (r, m) =>
            {
                _placeConflictFinderModels = m.Value;
                ReloadSchedule();
            });

            WeakReferenceMessenger.Default.Register<ChoicedSessionVmMsgs.DelClassGroupChoiceMsg>(this, (r, m) =>
            {
                _classGroupModels = m.Value;
                ReloadSchedule();
            });

            _classGroupModels = new List<ClassGroupModel>();
            _conflictModels = new List<ConflictModel>();
            _placeConflictFinderModels = new List<PlaceConflictFinderModel>();

            Phase1_Monday = new();
            Phase1_Tuesday = new();
            Phase1_Wednesday = new();
            Phase1_Thursday = new();
            Phase1_Friday = new();
            Phase1_Saturday = new();
            Phase1_Sunday = new();

            Phase2_Monday = new();
            Phase2_Tuesday = new();
            Phase2_Wednesday = new();
            Phase2_Thursday = new();
            Phase2_Friday = new();
            Phase2_Saturday = new();
            Phase2_Sunday = new();

            week1 = new ObservableCollection<TimeBlock>[7]
            {
                Phase1_Sunday,
                Phase1_Monday,
                Phase1_Tuesday,
                Phase1_Wednesday,
                Phase1_Thursday,
                Phase1_Friday,
                Phase1_Saturday,
            };

            week2 = new ObservableCollection<TimeBlock>[7]
            {
                Phase2_Sunday,
                Phase2_Monday,
                Phase2_Tuesday,
                Phase2_Wednesday,
                Phase2_Thursday,
                Phase2_Friday,
                Phase2_Saturday,
            };

            Timelines = new();
            foreach (string timeline in Controls.Utils.TIME_LINES)
            {
                Timelines.Add(timeline);
            }
        }

        private void DivideSchoolClassesByPhases()
        {
            foreach (ClassGroupModel classGroupModel in _classGroupModels)
            {
                IEnumerable<SchoolClassModel> schoolClassModels;
                if (classGroupModel.IsSpecialClassGroup)
                {
                    schoolClassModels = classGroupModel
                    .CurrentSchoolClassModels
                    .Select(sc => GetSchoolClassModelCallback(sc.SchoolClass, classGroupModel.Color));
                } 
                else
                {
                    schoolClassModels = classGroupModel
                    .ClassGroup
                    .SchoolClasses
                    .Select(sc => GetSchoolClassModelCallback(sc, classGroupModel.Color));
                }
                

                foreach (var item in schoolClassModels)
                {
                    AddSchoolClassModel(item);
                } 
            }
        }

        private void AddSchoolClassModel(SchoolClassModel schoolClassModel)
        {
            IEnumerable<TimeBlock> timeBlocks = schoolClassModel.GetBlocks();
            Phase phase = schoolClassModel.SchoolClass.GetPhase();
            foreach (TimeBlock timeBlock in timeBlocks)
            {
                ReplaceTimeBlock(timeBlock, phase);
            }
        }

        private void ReplaceTimeBlock(TimeBlock timeBlock, Phase phase)
        {
            int dayIndex = (int)timeBlock.DayOfWeek;
            if (phase != Phase.Non)
            {
                ObservableCollection<TimeBlock>[] week = phase == Phase.First ? week1 : week2;
                ObservableCollection<TimeBlock> timeBlocks = week[dayIndex];

                AddTimeBlockToDay(timeBlock, dayIndex, timeBlocks, week);
            }
            else if (phase == Phase.All)
            {
                ObservableCollection<TimeBlock> timeBlocksFirst = week1[dayIndex];
                ObservableCollection<TimeBlock> timeBlocksSecond = week2[dayIndex];
                AddTimeBlockToDay(timeBlock, dayIndex, timeBlocksFirst, week1);
                AddTimeBlockToDay(timeBlock, dayIndex, timeBlocksSecond, week2);
            }
        }

        private void AddTimeBlockToDay(TimeBlock timeBlock, int dayIndex, ObservableCollection<TimeBlock> timeBlocks, ObservableCollection<TimeBlock>[] week)
        {
            TimeBlock alreadyTimeBlock = timeBlocks
                .Where(tb => tb.SubjectCode == timeBlock.SubjectCode && tb.ClassGroupName != timeBlock.ClassGroupName)
                .FirstOrDefault();

            if (alreadyTimeBlock != null)
            {
                int timeBlockIndex = timeBlocks.IndexOf(alreadyTimeBlock);
                timeBlocks.RemoveAt(timeBlockIndex);
            }

            week[dayIndex].Add(timeBlock);
        }

        private void DivideConflictByPhase()
        {
            foreach (ConflictModel conflict in _conflictModels)
            {
                //if (conflict.GetPhase() == Phase.First || conflict.GetPhase() == Phase.All)
                //{
                //    Phase1.Add(conflict);
                //}
                //else
                //{
                //    Phase2.Add(conflict);
                //}
            }
        }

        private void DividePlaceConflictByPhase()
        {
            foreach (PlaceConflictFinderModel conflict in _placeConflictFinderModels)
            {
                //if (conflict.GetPhase() == Phase.First || conflict.GetPhase() == Phase.All)
                //{
                //    Phase1.Add(conflict);
                //}
                //else
                //{
                //    Phase2.Add(conflict);
                //}
            }
        }

        private static SchoolClassModel GetSchoolClassModelCallback(SchoolClass schoolClass, string color)
        {
            SchoolClassModel schoolClassModel = new(schoolClass)
            {
                Color = color
            };
            return schoolClassModel;
        }

        private void ReloadSchedule()
        {
            DivideSchoolClassesByPhases();
            DividePlaceConflictByPhase();
            DivideConflictByPhase();
        }
    }
}
