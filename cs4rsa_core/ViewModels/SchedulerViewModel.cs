using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.BaseClasses;
using Cs4rsa.Messages.Publishers;
using Cs4rsa.Messages.States;
using Cs4rsa.Services.ConflictSvc.Models;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes.Enums;
using Cs4rsa.Services.SubjectCrawlerSvc.Models;
using Cs4rsa.Utils;
using Cs4rsa.Utils.Interfaces;
using Cs4rsa.Utils.Models;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Cs4rsa.ViewModels
{
    /// <summary>
    /// Đại diện cho vị trí của các ScheduleTableItem.
    /// </summary>
    internal record Location
    {
        public readonly Phase Phase;
        public readonly IEnumerable<DayOfWeek> PhaseFirst;
        public readonly IEnumerable<DayOfWeek> PhaseSecond;

        public Location(
            Phase phase,
            IEnumerable<DayOfWeek> phaseFirst,
            IEnumerable<DayOfWeek> phaseSecond
        )
        {
            Phase = phase;
            PhaseFirst = phaseFirst;
            PhaseSecond = phaseSecond;
        }
    }

    /// <summary>
    /// Đại diện cho bản đồ ScheduleTableItem trên bộ mô phỏng.
    /// 
    /// Giúp thực hiện xác định chính xác vị trí của từ ScheduleTableItem
    /// có mặt trên mô phỏng. Hỗ trợ thao tác XOÁ.
    /// </summary>
    internal sealed class ScheduleItemMap
    {
        private readonly List<SchoolClassModel> _schoolClassModels;
        private readonly Dictionary<ScheduleItemId, Location> _map;

        public ScheduleItemMap()
        {
            _schoolClassModels = new();
            _map = new();
        }

        private void RemoveScmBySubejctCode(string subjectCode)
        {
            for (int i = 0; i < _schoolClassModels.Count;)
            {
                if (_schoolClassModels[i].SubjectCode.Equals(subjectCode))
                {
                    _schoolClassModels.RemoveAt(i);
                    continue;
                }
                else
                {
                    i++;
                }
            }
        }

        private void AddScmFromCgm(ClassGroupModel cgm)
        {
            RemoveScmBySubejctCode(cgm.SubjectCode);
            _schoolClassModels.AddRange(cgm.CurrentSchoolClassModels);
        }

        public void AddScheduleItem(SchoolClassModel scm)
        {
            List<DayOfWeek> phase1 = new();
            List<DayOfWeek> phase2 = new();

            Phase phase = scm.GetPhase();
            if (phase == Phase.All)
            {
                phase1.AddRange(scm.Schedule.GetSchoolDays());
                phase2.AddRange(scm.Schedule.GetSchoolDays());
            }
            else if (phase == Phase.First)
            {
                phase1.AddRange(scm.Schedule.GetSchoolDays());
            }
            else if (phase == Phase.Second)
            {
                phase2.AddRange(scm.Schedule.GetSchoolDays());
            }

            Location location = new
            (
                phase,
                phase1.Distinct(),
                phase2.Distinct()
            );

            _schoolClassModels.Add(scm);
            _map.Add(ScheduleItemId.Of(scm), location);
        }

        /// <summary>
        /// Phân rã ClassGroupModel thành các SchoolClassModel và thêm vào Map.
        /// </summary>
        /// <param name="classGroupModel"></param>
        public void AddScheduleItem(ClassGroupModel classGroupModel)
        {
            AddScmFromCgm(classGroupModel);
            foreach (SchoolClassModel scm in classGroupModel.CurrentSchoolClassModels)
            {
                List<DayOfWeek> phase1 = new();
                List<DayOfWeek> phase2 = new();

                Phase phase = scm.GetPhase();
                if (phase == Phase.All)
                {
                    phase1.AddRange(scm.Schedule.GetSchoolDays());
                    phase2.AddRange(scm.Schedule.GetSchoolDays());
                }
                else if (phase == Phase.First)
                {
                    phase1.AddRange(scm.Schedule.GetSchoolDays());
                }
                else if (phase == Phase.Second)
                {
                    phase2.AddRange(scm.Schedule.GetSchoolDays());
                }

                Location location = new
                (
                    phase,
                    phase1.Distinct(),
                    phase2.Distinct()
                );

                _map.Add(ScheduleItemId.Of(scm), location);
            }
        }

        public bool ExistsBySameSpace(ClassGroupModel classGroupModel)
        {
            foreach (SchoolClassModel scm in classGroupModel.CurrentSchoolClassModels)
            {
                var id = ScheduleItemId.Of(scm);
                foreach (ScheduleItemId key in _map.Keys)
                {
                    if (key.IsSameSpace(id)) return true;
                }
            }
            return false;
        }

        public bool ExistsBySameSpace(SubjectModel subjectModel)
        {
            foreach (ScheduleItemId key in _map.Keys)
            {
                if (key.Space.Equals(subjectModel.SubjectCode)) return true;
            }
            return false;
        }

        public bool ExistsById(ClassGroupModel classGroupModel)
        {
            foreach (SchoolClassModel scm in classGroupModel.CurrentSchoolClassModels)
            {
                var id = ScheduleItemId.Of(scm);
                foreach (ScheduleItemId key in _map.Keys)
                {
                    if (key.Equals(id)) return true;
                }
            }
            return false;
        }

        public IEnumerable<ScheduleItemId> GetScheduleItemID(ClassGroupModel cgm)
        {
            foreach (ScheduleItemId key in _map.Keys)
            {
                if (key.Space.Equals(cgm.SubjectCode))
                {
                    yield return key;
                }
            }
        }

        public IEnumerable<ScheduleItemId> GetScheduleItemId(SubjectModel subjectModel)
        {
            foreach (ScheduleItemId key in _map.Keys)
            {
                if (key.Space.Equals(subjectModel.SubjectCode))
                {
                    yield return key;
                }
            }
        }

        public Location GetLocation(ScheduleItemId id)
        {
            return _map[id];
        }

        public IEnumerable<Location> GetLocationsBySpace(ScheduleItemId id)
        {
            foreach (ScheduleItemId key in _map.Keys)
            {
                if (key.IsSameSpace(id))
                {
                    yield return _map[id];
                }
            }
        }

        public void Remove(ScheduleItemId id)
        {
            for (int i = 0; i < _schoolClassModels.Count; i++)
            {
                if (ScheduleItemId.Of(_schoolClassModels[i]).Equals(id))
                {
                    _schoolClassModels.RemoveAt(i);
                    break;
                }
            }
            _map.Remove(id);
        }

        public void RemoveBySpace(string space)
        {
            IEnumerable<ScheduleItemId> ids = _map.Keys.Where(k => k.Space.Equals(space));
            foreach (ScheduleItemId id in ids)
            {
                Remove(id);
            }
        }

        public void Clear()
        {
            _map.Clear();
        }

        internal IEnumerable<KeyValuePair<ScheduleItemId, Location>> GetMap()
        {
            return _map;
        }
    }

    internal sealed class SchedulerViewModel : ViewModelBase
    {
        private readonly ScheduleItemMap _map;
        private readonly ObservableCollection<ObservableCollection<TimeBlock>>[] _schedules;

        private readonly List<ClassGroupModel> _classGroupModels;
        private List<ConflictModel> _conflictModels;
        private List<PlaceConflictFinderModel> _placeConflictFinderModels;

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
            #region Fields
            _phaseStore = phaseStore;
            _map = new();
            _classGroupModels = new List<ClassGroupModel>();
            _conflictModels = new List<ConflictModel>();
            _placeConflictFinderModels = new List<PlaceConflictFinderModel>();
            #endregion

            #region WeakReferenceMessengers
            Messenger.Register<SearchVmMsgs.SelectCgmsMsg>(this, (r, m) =>
            {
                Trace.WriteLine("Messenger.Register<SearchVmMsgs.SelectCgmsMsg>");
                IEnumerable<ClassGroupModel> classes = m.Value;
                foreach (ClassGroupModel c in classes)
                {
                    IEnumerable<ScheduleItemId> scheduleItemIds = ScheduleItemId.Of(c);
                    IEnumerable<ScheduleItemId> sameSpaceScheduleItemIds = _map.GetScheduleItemID(c);
                    if (!scheduleItemIds.Intersect(sameSpaceScheduleItemIds).Any())
                    {
                        RemoveClassGroup(c);
                        AddClassGroup(c);
                    }
                }
            });

            Messenger.Register<SearchVmMsgs.DelAllSubjectMsg>(this, (r, m) =>
            {
                CleanAll();
            });

            Messenger.Register<SearchVmMsgs.DelSubjectMsg>(this, (r, m) =>
            {
                if (_map.ExistsBySameSpace(m.Value))
                {
                    RemoveSubjectModel(m.Value);
                }
            });

            Messenger.Register<ClassGroupSessionVmMsgs.ClassGroupAddedMsg>(this, (r, m) =>
            {
                Trace.WriteLine("Messenger.Register<ClassGroupSessionVmMsgs.ClassGroupAddedMsg>");
                ClassGroupModel classGroupModel = m.Value;
                IEnumerable<ScheduleItemId> scheduleItemIds = ScheduleItemId.Of(classGroupModel);
                IEnumerable<ScheduleItemId> sameSpaceScheduleItemIds = _map.GetScheduleItemID(classGroupModel);
                if (!scheduleItemIds.Intersect(sameSpaceScheduleItemIds).Any())
                {
                    RemoveClassGroup(classGroupModel);
                    AddClassGroup(classGroupModel);
                }
            });

            Messenger.Register<ChoosedVmMsgs.UndoDelAllMsg>(this, (r, m) =>
            {
                Trace.WriteLine("StrongReferenceMessenger.Default.Register<ChoosedVmMsgs.UndoDelAllMsg>");
                IEnumerable<ClassGroupModel> classGroupModels = m.Value;
                foreach (ClassGroupModel cgm in classGroupModels)
                {
                    AddClassGroup(cgm);
                }
            });

            Messenger.Register<ChoosedVmMsgs.ConflictCollChangedMsg>(this, (r, m) =>
            {
                _conflictModels = m.Value.ToList();
            });

            Messenger.Register<ChoosedVmMsgs.PlaceConflictCollChangedMsg>(this, (r, m) =>
            {
                _placeConflictFinderModels = m.Value.ToList();
            });

            Messenger.Register<ChoosedVmMsgs.DelClassGroupChoiceMsg>(this, (r, m) =>
            {
                ClassGroupModel classGroupModel = m.Value;
                if (_map.ExistsBySameSpace(classGroupModel))
                {
                    RemoveClassGroup(classGroupModel);
                }
            });

            Messenger.Register<ChoosedVmMsgs.DelAllClassGroupChoiceMsg>(this, (r, m) =>
            {
                CleanAll();
            });

            Messenger.Register<PhaseStoreMsgs.BetweenPointChangedMsg>(this, (r, m) =>
            {
                DivideSchoolClassesByPhases();
                DividePlaceConflictByPhase();
                DivideConflictByPhase();
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

            _schedules = new[]
            {
                Week1,
                Week2,
            };

            Timelines = new();
            foreach (string timeline in Controls.Utils.TIME_LINES)
            {
                Timelines.Add(timeline);
            }
            #endregion
        }

        private void RemoveSubjectModel(SubjectModel subjectModel)
        {
            ClassGroupModel classGroupModel = _classGroupModels
                .Where(cgm => cgm.SubjectCode.Equals(subjectModel.SubjectCode))
                .FirstOrDefault();
            if (classGroupModel != null)
            {
                RemoveClassGroup(classGroupModel);
            }
        }

        /// <summary>
        /// Kiểm tra và nếu tồn tại thì loại bỏ hoàn toàn một <see cref="ClassGroupModel"/> ra khỏi mô phỏng.
        /// </summary>
        /// <param name="classGroupModel">ClassGroupModel</param>
        private void RemoveClassGroup(ClassGroupModel classGroupModel)
        {
            if (!_map.ExistsBySameSpace(classGroupModel))
            {
                return;
            }

            #region Remove from Simulator
            IEnumerable<ScheduleItemId> scheduleItemIds = _map.GetScheduleItemID(classGroupModel);
            foreach (ScheduleItemId id in scheduleItemIds)
            {
                Location location = _map.GetLocation(id);
                RemoveScheduleItem(id, location);
            }
            #endregion

            #region Remove from manager
            for (int i = 0; i < _classGroupModels.Count; i++)
            {
                if (_classGroupModels[i].SubjectCode.Equals(classGroupModel.SubjectCode))
                {
                    _classGroupModels.RemoveAt(i);
                    break;
                }
            }
            #endregion

            #region Remove from map
            _map.RemoveBySpace(classGroupModel.SubjectCode);
            #endregion
        }

        /// <summary>
        /// Loại bỏ ScheduleItem khỏi mô phỏng.
        /// 
        /// </summary>
        /// <param name="id">ScheduleItemId</param>
        /// <param name="location">Location</param>
        private void RemoveScheduleItem(ScheduleItemId id, Location location)
        {
            if (location.PhaseFirst.Any())
            {
                foreach (DayOfWeek dayOfWeek in location.PhaseFirst)
                {
                    RemoveScheduleItemBySpace(id, Week1[dayOfWeek.ToIndex()]);
                }
            }

            if (location.PhaseSecond.Any())
            {
                foreach (DayOfWeek dayOfWeek in location.PhaseSecond)
                {
                    RemoveScheduleItemBySpace(id, Week2[dayOfWeek.ToIndex()]);
                }
            }
        }

        /// <summary>
        /// Remove schedule item with same cgm
        /// </summary>
        /// <param name="scheduleItemID"></param>
        /// <param name="day"></param>
        private static void RemoveScheduleItemBySpace(ScheduleItemId scheduleItemID, ObservableCollection<TimeBlock> day)
        {
            if (day.Count == 0) return;
            int currentIndex = 0;
            bool isEndOfList = false;
            while (!isEndOfList)
            {
                if (day[currentIndex].ScheduleTableItemType == ScheduleTableItemType.SchoolClass
                    && day[currentIndex].Id.IsSameSpace(scheduleItemID))
                {
                    day.RemoveAt(currentIndex);
                }
                isEndOfList = currentIndex >= day.Count - 1;
                currentIndex++;
            }
        }

        /// <summary>
        /// Thay thế ClassGroupModel cũ trong bộ mô phỏng (nếu có)
        /// bằng ClassGroupModel mới được thêm.
        /// </summary>
        /// <param name="classGroupModel"></param>
        private void AddClassGroup(ClassGroupModel classGroupModel)
        {
            #region Add to Map
            _map.AddScheduleItem(classGroupModel);
            #endregion

            #region Add to Manager
            _classGroupModels.Add(classGroupModel);
            #endregion

            #region Add to Simulator
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
            #endregion
        }

        /// <summary>
        /// Thực hiện vẽ lại _map và mô phỏng.
        /// 
        /// Mô tả:
        ///     Quét lại _map, đánh giá lại Phase, 
        ///     remove và vẽ lại các SchoolClass có Phase thay đổi.
        /// </summary>
        private void DivideSchoolClassesByPhases()
        {
            IEnumerable<KeyValuePair<ScheduleItemId, Location>> map = _map.GetMap().ToList();
            foreach (KeyValuePair<ScheduleItemId, Location> locationItem in map)
            {
                ClassGroupModel classGroupModel = _classGroupModels.Find(cgm => cgm.SubjectCode.Equals(locationItem.Key.Space));

                if (classGroupModel == null) continue;

                IEnumerable<SchoolClassModel> schoolClassModels = classGroupModel.CurrentSchoolClassModels;
                foreach (SchoolClassModel scm in schoolClassModels)
                {
                    Phase oldPhase = scm.Phase;
                    Phase newPhase = scm.GetPhase();

                    if (oldPhase == newPhase) continue;

                    ScheduleItemId id = ScheduleItemId.Of(scm);
                    _map.Remove(id);
                    _map.AddScheduleItem(scm);
                    AddScheduleItem(scm);
                }
            }
        }

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

        private void DivideConflictByPhase()
        {
            foreach (ConflictModel conflictModel in _conflictModels)
            {
                AddScheduleItem(conflictModel);
            }
        }

        private void DividePlaceConflictByPhase()
        {
            foreach (PlaceConflictFinderModel conflictFinderModel in _placeConflictFinderModels)
            {
                AddScheduleItem(conflictFinderModel);
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

        private void CleanAll()
        {
            _classGroupModels.Clear();
            _map.Clear();
            _conflictModels.Clear();
            _placeConflictFinderModels.Clear();
            CleanDays();
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
    }
}
