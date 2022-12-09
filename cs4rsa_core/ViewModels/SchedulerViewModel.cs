using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.BaseClasses;
using Cs4rsa.Interfaces;
using Cs4rsa.Messages.Publishers;
using Cs4rsa.Messages.States;
using Cs4rsa.Services.ConflictSvc.Models;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes.Enums;
using Cs4rsa.Services.SubjectCrawlerSvc.Models;
using Cs4rsa.Utils;
using Cs4rsa.Utils.Models;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Cs4rsa.ViewModels
{
    /// <summary>
    /// Phải THẬT biết những gì mình biết. Thế mới là HỌC.
    /// 
    /// SchedulerViewModel chả quản lý cái quần gì hết. Nó chỉ hiển thị thôi.
    /// </summary>
    internal sealed class SchedulerViewModel : ViewModelBase
    {
        private readonly ObservableCollection<ObservableCollection<TimeBlock>>[] _schedules;

        #region Properties
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

        public ObservableCollection<ObservableCollection<TimeBlock>> Week1 { get; set; }
        public ObservableCollection<ObservableCollection<TimeBlock>> Week2 { get; set; }

        public ObservableCollection<string> Timelines { get; set; }

        public PhaseStore PhaseStore
        {
            get
            {
                return _phaseStore;
            }
        }
        #endregion

        #region Commands
        public RelayCommand ResetBetweenPointCommand { get; set; }
        #endregion

        #region DI
        private readonly PhaseStore _phaseStore;
        #endregion

        public SchedulerViewModel(PhaseStore phaseStore)
        {
            _phaseStore = phaseStore;

            #region Messengers
            Messenger.Register<SearchVmMsgs.SelectCgmsMsg>(this, (r, m) =>
            {
                IEnumerable<ClassGroupModel> classes = m.Value;
                foreach (ClassGroupModel c in classes)
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

            Messenger.Register<ClassGroupSessionVmMsgs.ClassGroupAddedMsg>(this, (r, m) =>
            {
                ClassGroupModel classGroupModel = m.Value;
                RemoveScheduleItem(classGroupModel.SubjectCode);
                AddClassGroup(classGroupModel);
            });

            Messenger.Register<ChoosedVmMsgs.UndoDelAllMsg>(this, (r, m) =>
            {
                IEnumerable<ClassGroupModel> classGroupModels = m.Value;
                foreach (ClassGroupModel cgm in classGroupModels)
                {
                    AddClassGroup(cgm);
                }
            });


            // Không remove những thằng đã tồn tại.
            // Remove những thằng không tồn tại
            // Thêm mới những thằng mới
            Messenger.Register<ChoosedVmMsgs.ConflictCollChangedMsg>(this, (r, m) =>
            {
                List<ConflictModel> conflictModels = m.Value.ToList();
                IEnumerable<string> conflictIds = conflictModels
                    .Select(cm => cm.GetId())
                    .ToList();
                RemoveConflictNotInContains(conflictIds);
                AddNewConflicts(conflictModels);
            });

            Messenger.Register<ChoosedVmMsgs.PlaceConflictCollChangedMsg>(this, (r, m) =>
            {
                List<PlaceConflictFinderModel> placeConflicts = m.Value.ToList();
                IEnumerable<string> conflictIds = placeConflicts
                    .Select(cm => cm.GetId())
                    .ToList();
                RemoveConflictNotInContains(conflictIds);
                AddNewConflicts(placeConflicts);
            });

            Messenger.Register<ChoosedVmMsgs.DelClassGroupChoiceMsg>(this, (r, m) =>
            {
                ClassGroupModel classGroupModel = m.Value;
            });

            Messenger.Register<ChoosedVmMsgs.DelAllClassGroupChoiceMsg>(this, (r, m) =>
            {
                CleanDays();
            });

            Messenger.Register<PhaseStoreMsgs.BetweenPointChangedMsg>(this, (r, m) =>
            {
                ChoosedSessionViewModel choosedSessionViewModel = GetViewModel<ChoosedSessionViewModel>();
                ObservableCollection<ClassGroupModel> classGroupModels = choosedSessionViewModel.ClassGroupModels;
                ObservableCollection<ConflictModel> conflictModels = choosedSessionViewModel.ConflictModels;
                ObservableCollection<PlaceConflictFinderModel> placeConflicts = choosedSessionViewModel.PlaceConflictFinderModels;

                CleanDays();
                foreach (ClassGroupModel classGroupModel in classGroupModels)
                {
                    AddClassGroup(classGroupModel);
                }
                foreach (ConflictModel conflictModel in conflictModels)
                {
                    AddScheduleItem(conflictModel);
                }
                foreach (PlaceConflictFinderModel placeConflict in placeConflicts)
                {
                    AddScheduleItem(placeConflict);
                }
            });
            #endregion

            #region Commands
            ResetBetweenPointCommand = new(() => _phaseStore.EvaluateBetweenPoint());
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

            Timelines = new();
            foreach (string timeline in Controls.Utils.TIME_LINES)
            {
                Timelines.Add(timeline);
            }
            #endregion
        }

        private void AddNewConflicts(List<ConflictModel> scheduleTableItems)
        {
            scheduleTableItems = scheduleTableItems.Where(sti => !Exists(sti)).ToList();
            scheduleTableItems.ForEach(sti => AddScheduleItem(sti));
        }

        private void AddNewConflicts(List<PlaceConflictFinderModel> scheduleTableItems)
        {
            scheduleTableItems = scheduleTableItems.Where(sti => !Exists(sti)).ToList();
            scheduleTableItems.ForEach(sti => AddScheduleItem(sti));
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
                    while (day.Any())
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
        /// <param name="classGroupModel"></param>
        private void AddClassGroup(ClassGroupModel classGroupModel)
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

            foreach (SchoolClassModel schoolClassModel in schoolClassModels)
            {
                AddScheduleItem(schoolClassModel);
            }
        }

        /// <summary>
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
                    ObservableCollection<ObservableCollection<TimeBlock>> week = phase == Phase.First
                                                           ? Week1
                                                           : Week2;
                    week[dayIndex].Add(timeBlock);
                }
                else if (phase == Phase.All)
                {
                    Week1[dayIndex].Add(timeBlock);
                    Week2[dayIndex].Add(timeBlock);
                }
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

        private void RemoveConflictNotInContains(IEnumerable<string> conflictIds)
        {
            foreach (ObservableCollection<ObservableCollection<TimeBlock>> week in _schedules)
            {
                foreach (ObservableCollection<TimeBlock> day in week)
                {
                    int currentIndex = 0;
                    while (day.Any())
                    {
                        if (!conflictIds.Contains(day[currentIndex].Id))
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
