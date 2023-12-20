using Cs4rsa.Common;
using Cs4rsa.Messages.Publishers;
using Cs4rsa.Module.ManuallySchedule.Models;
using Cs4rsa.Service.Conflict.DataTypes.Enums;
using Cs4rsa.Service.SubjectCrawler.DataTypes.Enums;
using Cs4rsa.UI.ScheduleTable.Interfaces;
using Cs4rsa.UI.ScheduleTable.Models;

using Prism.Events;
using Prism.Mvvm;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Cs4rsa.Module.ManuallySchedule.Events;

namespace Cs4rsa.Module.ManuallySchedule.ViewModels
{
    public class SchedulerViewModel : BindableBase
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

        private IEventAggregator _eventAggregator;
        public SchedulerViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            eventAggregator.GetEvent<SearchVmMsgs.SelectCgmsMsg>().Subscribe(payload =>
            {
                foreach (var c in payload)
                {
                    RemoveScheduleItem(c.SubjectCode);
                    AddClassGroup(c);
                }
            });

            eventAggregator.GetEvent<SearchVmMsgs.DelAllSubjectMsg>().Subscribe(CleanDays);

            eventAggregator.GetEvent<SearchVmMsgs.DelSubjectMsg>().Subscribe(payload => RemoveScheduleItem(payload.SubjectCode));

            eventAggregator.GetEvent<SearchVmMsgs.DelSubjectMsg>().Subscribe(payload => RemoveScheduleItem(payload.SubjectCode));

            eventAggregator.GetEvent<ChoosedVmMsgs.ClassGroupAddedMsg>().Subscribe(payload =>
            {
                RemoveScheduleItem(payload.SubjectCode);
                AddClassGroup(payload);
            });

            eventAggregator.GetEvent<ChoosedVmMsgs.UndoDelAllMsg>().Subscribe(payload =>
            {
                var classGroupModels = payload.Item1;
                var conflictModels = payload.Item2;
                var placeConflicts = payload.Item3;

                foreach (var cgm in classGroupModels)
                {
                    AddClassGroup(cgm);
                }
                foreach (var conflictModel in conflictModels)
                {
                    AddScheduleItem(conflictModel);
                }
                foreach (var placeConflict in placeConflicts)
                {
                    AddScheduleItem(placeConflict);
                }
            });

            eventAggregator.GetEvent<ChoosedVmMsgs.ConflictCollChangedMsg>().Subscribe(payload =>
            {
                var conflictIds = payload.Select(cm => cm.GetId());
                RemoveConflictNotInContains(conflictIds, ConflictType.Time);
                AddNewConflicts(payload);
            });

            eventAggregator.GetEvent<ChoosedVmMsgs.PlaceConflictCollChangedMsg>().Subscribe(payload =>
            {
                var conflictIds = payload.Select(cm => cm.GetId());
                RemoveConflictNotInContains(conflictIds, ConflictType.Place);
                AddNewConflicts(payload);
            });

            eventAggregator.GetEvent<ChoosedVmMsgs.DelClassGroupChoiceMsg>().Subscribe(payload =>
            {
                foreach (var scm in payload.CurrentSchoolClassModels)
                {
                    RemoveScheduleItem(scm.SubjectCode);
                }
            });

            eventAggregator.GetEvent<ChoosedVmMsgs.DelAllClassGroupChoiceMsg>().Subscribe(payload =>
            {
                CleanDays();
            });

            eventAggregator.GetEvent<ChoosedVmMsgs.UndoDelMsg>().Subscribe(AddClassGroup);

            //eventAggregator.GetEvent<UpdateVmMsgs.UpdateSuccessMsg>().Subscribe(payload =>
            //{
            //    CleanDays();
            //});

            #region Weeks and Timelines
            Phase1_Monday = new ObservableCollection<TimeBlock>();
            Phase1_Tuesday = new ObservableCollection<TimeBlock>();
            Phase1_Wednesday = new ObservableCollection<TimeBlock>();
            Phase1_Thursday = new ObservableCollection<TimeBlock>();
            Phase1_Friday = new ObservableCollection<TimeBlock>();
            Phase1_Saturday = new ObservableCollection<TimeBlock>();
            Phase1_Sunday = new ObservableCollection<TimeBlock>();
            Phase2_Monday = new ObservableCollection<TimeBlock>();
            Phase2_Tuesday = new ObservableCollection<TimeBlock>();
            Phase2_Wednesday = new ObservableCollection<TimeBlock>();
            Phase2_Thursday = new ObservableCollection<TimeBlock>();
            Phase2_Friday = new ObservableCollection<TimeBlock>();
            Phase2_Saturday = new ObservableCollection<TimeBlock>();
            Phase2_Sunday = new ObservableCollection<TimeBlock>();

            Week1 = new ObservableCollection<ObservableCollection<TimeBlock>>()
            {
                Phase1_Monday,
                Phase1_Tuesday,
                Phase1_Wednesday,
                Phase1_Thursday,
                Phase1_Friday,
                Phase1_Saturday,
                Phase1_Sunday
            };

            Week2 = new ObservableCollection<ObservableCollection<TimeBlock>>()
            {
                Phase2_Monday,
                Phase2_Tuesday,
                Phase2_Wednesday,
                Phase2_Thursday,
                Phase2_Friday,
                Phase2_Saturday,
                Phase2_Sunday
            };

            _schedules = new List<ObservableCollection<ObservableCollection<TimeBlock>>>() { Week1, Week2 };

            Timelines = new ObservableCollection<string>();
            foreach (var timeline in Cs4rsa.UI.ScheduleTable.Utils.Utils.TimeLines)
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
            foreach (var week in _schedules)
            {
                foreach (var day in week)
                {
                    foreach (var timeBlock in day)
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
            foreach (var week in _schedules)
            {
                foreach (var day in week)
                {
                    var currentIndex = 0;
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

            foreach (var schoolClassModel in schoolClassModels)
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
            var timeBlocks = scheduleItem.GetBlocks();
            var phase = scheduleItem.GetPhase();
            foreach (var timeBlock in timeBlocks)
            {
                var dayIndex = timeBlock.DayOfWeek.ToIndex();
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
            foreach (var week in _schedules)
            {
                foreach (var day in week)
                {
                    var currentIndex = 0;
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
