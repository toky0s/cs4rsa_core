using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Cs4rsa.Module.ManuallySchedule.ViewModels
{
    internal sealed partial class SchedulerViewModel : ViewModelBase
    {
        private readonly List<ObservableCollection<ObservableCollection<TimeBlock>>> _schedules;

        #region Properties
        public ObservableCollection<TimeBlock> Phase1_Monday { get; }
        public ObservableCollection<TimeBlock> Phase1_Tuesday { get; }
        public ObservableCollection<TimeBlock> Phase1_Wednesday { get; }
        public ObservableCollection<TimeBlock> Phase1_Thursday { get; }
        public ObservableCollection<TimeBlock> Phase1_Friday { get; }
        public ObservableCollection<TimeBlock> Phase1_Saturday { get; }
        public ObservableCollection<TimeBlock> Phase1_Sunday { get; }
        public ObservableCollection<TimeBlock> Phase2_Monday { get; }
        public ObservableCollection<TimeBlock> Phase2_Tuesday { get; }
        public ObservableCollection<TimeBlock> Phase2_Wednesday { get; }
        public ObservableCollection<TimeBlock> Phase2_Thursday { get; }
        public ObservableCollection<TimeBlock> Phase2_Friday { get; }
        public ObservableCollection<TimeBlock> Phase2_Saturday { get; }
        public ObservableCollection<TimeBlock> Phase2_Sunday { get; }

        public ObservableCollection<ObservableCollection<TimeBlock>> Week1 { get; }
        public ObservableCollection<ObservableCollection<TimeBlock>> Week2 { get; }

        public ObservableCollection<string> Timelines { get; }

        #endregion

        public SchedulerViewModel()
        {

            #region Messengers
            Messenger.Register<SearchVmMsgs.SelectCgmsMsg>(this, (r, m) =>
            {
                foreach (ClassGroupModel c in m.Value)
                {
                    RemoveScheduleItem(c.SubjectCode);
                    AddClassGroup(c);
                }
            });

            Messenger.Register<SearchVmMsgs.DelAllSubjectMsg>(this, (r, m) =>
            {
                CleanDays();
            });

            Messenger.Register<SearchVmMsgs.DelSubjectMsg>(this, (r, m) =>
            {
                RemoveScheduleItem(m.Value.SubjectCode);
            });

            Messenger.Register<ChoosedVmMsgs.ClassGroupAddedMsg>(this, (r, m) =>
            {
                ClassGroupModel classGroupModel = m.Value;
                RemoveScheduleItem(m.Value.SubjectCode);
                AddClassGroup(classGroupModel);
            });

            Messenger.Register<ChoosedVmMsgs.UndoDelAllMsg>(this, (r, m) =>
            {
                IEnumerable<ClassGroupModel> classGroupModels = m.Value;
                foreach (ClassGroupModel cgm in classGroupModels)
                {
                    AddClassGroup(cgm);
                }
                ChoseViewModel choseSessionViewModel = GetViewModel<ChoseViewModel>();
                ObservableCollection<ConflictModel> conflictModels = choseSessionViewModel.ConflictModels;
                ObservableCollection<PlaceConflictFinderModel> placeConflicts = choseSessionViewModel.PlaceConflictFinderModels;
                foreach (ConflictModel conflictModel in conflictModels)
                {
                    AddScheduleItem(conflictModel);
                }
                foreach (PlaceConflictFinderModel placeConflict in placeConflicts)
                {
                    AddScheduleItem(placeConflict);
                }
            });

            Messenger.Register<ChoosedVmMsgs.ConflictCollChangedMsg>(this, (r, m) =>
            {
                IEnumerable<string> conflictIds = m.Value.Select(cm => cm.GetId());
                RemoveConflictNotInContains(conflictIds, ConflictType.Time);
                AddNewConflicts(m.Value);
            });

            Messenger.Register<ChoosedVmMsgs.PlaceConflictCollChangedMsg>(this, (r, m) =>
            {
                IEnumerable<string> conflictIds = m.Value.Select(cm => cm.GetId());
                RemoveConflictNotInContains(conflictIds, ConflictType.Place);
                AddNewConflicts(m.Value);
            });

            Messenger.Register<ChoosedVmMsgs.DelClassGroupChoiceMsg>(this, (r, m) =>
            {
                ClassGroupModel classGroupModel = m.Value;
                foreach (SchoolClassModel scm in classGroupModel.CurrentSchoolClassModels)
                {
                    RemoveScheduleItem(scm.SubjectCode);
                }
            });

            Messenger.Register<ChoosedVmMsgs.DelAllClassGroupChoiceMsg>(this, (r, m) =>
            {
                CleanDays();
            });

            Messenger.Register<ChoosedVmMsgs.UndoDelMsg>(this, (r, m) =>
            {
                AddClassGroup(m.Value);
            });

            Messenger.Register<UpdateVmMsgs.UpdateSuccessMsg>(this, (r, m) =>
            {
                CleanDays();
            });
            #endregion

            #region Weeks and Timelines
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

            Week1 = new()
            {
                Phase1_Monday,
                Phase1_Tuesday,
                Phase1_Wednesday,
                Phase1_Thursday,
                Phase1_Friday,
                Phase1_Saturday,
                Phase1_Sunday
            };

            Week2 = new()
            {
                Phase2_Monday,
                Phase2_Tuesday,
                Phase2_Wednesday,
                Phase2_Thursday,
                Phase2_Friday,
                Phase2_Saturday,
                Phase2_Sunday
            };

            _schedules = new() { Week1, Week2 };

            Timelines = new();
            foreach (string timeline in Controls.Utils.TIME_LINES)
            {
                Timelines.Add(timeline);
            }
            #endregion
        }

        private void AddNewConflicts(IEnumerable<IScheduleTableItem> scheduleTableItems)
        {
            scheduleTableItems = scheduleTableItems.Where(sti => !Exists(sti));
            foreach (var sti in scheduleTableItems)
            {
                AddScheduleItem(sti);
            }
        }

        private bool Exists(IScheduleTableItem item)
        {
            foreach (ObservableCollection<ObservableCollection<TimeBlock>> week in _schedules)
            {
                foreach (ObservableCollection<TimeBlock> day in week)
                {
                    foreach (TimeBlock timeBlock in day)
                    {
                        if (timeBlock.Id == item.GetId()) return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Loại bỏ ScheduleItem khỏi mô phỏng.
        /// 
        /// </summary>
        /// <param name="id">
        /// ID với SchoolClassModel sẽ là Subject Code của nó.
        /// ID của các Conflict sẽ là sự kết hợp giữa hai tên SchoolClassModel.
        /// </param>
        private void RemoveScheduleItem(string id)
        {
            foreach (ObservableCollection<ObservableCollection<TimeBlock>> week in _schedules)
            {
                foreach (ObservableCollection<TimeBlock> day in week)
                {
                    int currentIndex = 0;
                    while (currentIndex < day.Count)
                    {
                        if (id == day[currentIndex].Id)
                        {
                            day.RemoveAt(currentIndex);
                            continue;
                        }
                        currentIndex++;
                    }
                }
            }
        }

        /// <summary>
        /// Thay thế ClassGroupModel cũ trong bộ mô phỏng (nếu có)
        /// bằng ClassGroupModel mới được thêm.
        /// </summary>
        /// <param name="classGroupModel">ClassGroupModel</param>
        private void AddClassGroup(ClassGroupModel classGroupModel)
        {
            IEnumerable<SchoolClassModel> schoolClassModels;
            if (classGroupModel.IsSpecialClassGroup)
            {
                schoolClassModels = classGroupModel.CurrentSchoolClassModels;
            }
            else
            {
                schoolClassModels = classGroupModel.NormalSchoolClassModels;
            }

            foreach (SchoolClassModel schoolClassModel in schoolClassModels)
            {
                AddScheduleItem(schoolClassModel);
            }
        }
        
        /// <summary>
        /// IMPORTANT!!!
        /// 
        /// Vẽ một <see cref="IScheduleTableItem"/> lên mô phỏng.
        /// </summary>
        /// <param name="scheduleItem">IScheduleTableItem</param>
        private void AddScheduleItem(IScheduleTableItem scheduleItem)
        {
            IEnumerable<TimeBlock> timeBlocks = scheduleItem.GetBlocks();
            Phase phase = scheduleItem.GetPhase();
            foreach (TimeBlock timeBlock in timeBlocks)
            {
                int dayIndex = timeBlock.DayOfWeek.ToIndex();
                if (phase == Phase.First || phase == Phase.Second)
                {
                    var week = phase == Phase.First ? Week1 : Week2;
                    week[dayIndex].Add(timeBlock);
                }
                else if (phase == Phase.All)
                {
                    Week1[dayIndex].Add(timeBlock);
                    Week2[dayIndex].Add(timeBlock);
                }
            }
        }

        private void CleanDays()
        {
            Phase1_Monday.Clear();
            Phase1_Tuesday.Clear();
            Phase1_Wednesday.Clear();
            Phase1_Thursday.Clear();
            Phase1_Friday.Clear();
            Phase1_Saturday.Clear();
            Phase1_Sunday.Clear();

            Phase2_Monday.Clear();
            Phase2_Tuesday.Clear();
            Phase2_Wednesday.Clear();
            Phase2_Thursday.Clear();
            Phase2_Friday.Clear();
            Phase2_Saturday.Clear();
            Phase2_Sunday.Clear();
        }

        private void RemoveConflictNotInContains(IEnumerable<string> conflictIds, ConflictType conflictType)
        {
            foreach (ObservableCollection<ObservableCollection<TimeBlock>> week in _schedules)
            {
                foreach (ObservableCollection<TimeBlock> day in week)
                {
                    int currentIndex = 0;
                    while (currentIndex < day.Count)
                    {
                        if (!conflictIds.Contains(day[currentIndex].Id)
                            && (day[currentIndex].ScheduleTableItemType == ScheduleTableItemType.TimeConflict
                            || day[currentIndex].ScheduleTableItemType == ScheduleTableItemType.PlaceConflict)
                            && day[currentIndex].Id.StartsWith(conflictType == ConflictType.Time ? "tc" : "pc"))
                        {
                            day.RemoveAt(currentIndex);
                            continue;
                        }
                        currentIndex++;
                    }
                }
            }
        }
    }
}
