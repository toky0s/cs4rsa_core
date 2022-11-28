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
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Cs4rsa.ViewModels
{
    /// <summary>
    /// Đại diện cho vị trí của các ScheduleTableItem.
    /// </summary>
    internal readonly struct Location
    {
        public readonly IEnumerable<DayOfWeek> Phase1;
        public readonly IEnumerable<DayOfWeek> Phase2;

        public Location(IEnumerable<DayOfWeek> phase1, IEnumerable<DayOfWeek> phase2)
        {
            Phase1 = phase1;
            Phase2 = phase2;
        }
    }

    /// <summary>
    /// Đại diện cho bản đồ ScheduleTableItem trên bộ mô phỏng.
    /// 
    /// Giúp thực hiện xác định chính xác vị trí của từ ScheduleTableItem
    /// có mặt trên mô phỏng. Hỗ trợ thao tác XOÁ.
    /// </summary>
    internal sealed class TimeBlockMap
    {
        public Dictionary<string, Location> Map { get; }
        public TimeBlockMap()
        {
            Map = new();
        }

        public void AddScheduleItem(ClassGroupModel classGroupModel)
        {
            string id = classGroupModel.SubjectCode;
            List<DayOfWeek> phase1 = new();
            List<DayOfWeek> phase2 = new();
            foreach (SchoolClassModel scm in classGroupModel.CurrentSchoolClassModels)
            {
                if(scm.GetPhase() == Phase.All)
                {
                    phase1.AddRange(scm.Schedule.GetSchoolDays());
                    phase2.AddRange(scm.Schedule.GetSchoolDays());
                }
                else if (scm.GetPhase() == Phase.First)
                {
                    phase1.AddRange(scm.Schedule.GetSchoolDays());
                }
                else if (scm.GetPhase() == Phase.Second)
                {
                    phase2.AddRange(scm.Schedule.GetSchoolDays());
                }
            }
            Location location = new
            (
                phase1.Distinct(),
                phase2.Distinct()
            );
            Map.Add(id, location);
        }

        public bool IsExist(ClassGroupModel classGroupModel)
        {
            if (classGroupModel.CurrentSchoolClassModels.Any())
            {
                return Map.ContainsKey(classGroupModel.CurrentSchoolClassModels[0].GetId());
            }
            return Map.ContainsKey(classGroupModel.SubjectCode);
        }
    }

    internal sealed class ScheduleTableViewModel : ViewModelBase
    {
        private readonly TimeBlockMap _timeBlockMap;
        private readonly ObservableCollection<ObservableCollection<TimeBlock>>[] _schedules;

        private List<ClassGroupModel> _classGroupModels;
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

        public ScheduleTableViewModel(PhaseStore phaseStore)
        {
            _phaseStore = phaseStore;
            _timeBlockMap = new();
            _classGroupModels = new List<ClassGroupModel>();
            _conflictModels = new List<ConflictModel>();
            _placeConflictFinderModels = new List<PlaceConflictFinderModel>();

            #region WeakReferenceMessengers
            WeakReferenceMessenger.Default.Register<ChoicedSessionVmMsgs.ChoiceChangedMsg>(this, (r, m) =>
            {
                foreach (ClassGroupModel classGroupModel in m.Value)
                {
                    if (_timeBlockMap.IsExist(classGroupModel))
                    {
                        RemoveClassGroup(classGroupModel);
                    }
                    _timeBlockMap.AddScheduleItem(classGroupModel);
                    AddClassGroup(classGroupModel);
                }
            });

            WeakReferenceMessenger.Default.Register<ChoicedSessionVmMsgs.ClassGroupSeletedMsg>(this, (r, m) =>
            {
                ClassGroupModel classGroupModel = m.Value;
                /// Check Map
                /// Tồn tại -> Lấy vị trí map -> Xoá
                /// Loại bỏ vị trí khỏi map
                /// Loại bỏ khỏi danh sách quản lý
                /// 
                /// Add vị trí mới vào map
                /// Add vào danh sách quản lý
                /// Add vào mô phỏng
                if (_timeBlockMap.IsExist(classGroupModel))
                {
                    RemoveClassGroup(classGroupModel);
                }
                _timeBlockMap.AddScheduleItem(classGroupModel);
                AddClassGroup(classGroupModel);
            });
            
            WeakReferenceMessenger.Default.Register<ChoicedSessionVmMsgs.ConflictCollChangedMsg>(this, (r, m) =>
            {
                _conflictModels = m.Value.ToList();
            });

            WeakReferenceMessenger.Default.Register<ChoicedSessionVmMsgs.PlaceConflictCollChangedMsg>(this, (r, m) =>
            {
                _placeConflictFinderModels = m.Value.ToList();
            });

            WeakReferenceMessenger.Default.Register<ChoicedSessionVmMsgs.DelClassGroupChoiceMsg>(this, (r, m) =>
            {
                ClassGroupModel classGroupModel = m.Value;
                if (_timeBlockMap.IsExist(classGroupModel))
                {
                    RemoveClassGroup(classGroupModel);
                }
            });
            
            WeakReferenceMessenger.Default.Register<ChoicedSessionVmMsgs.DelAllClassGroupChoiceMsg>(this, (r, m) =>
            {
                _classGroupModels.Clear();
                _timeBlockMap.Map.Clear();
                _conflictModels.Clear();
                _placeConflictFinderModels.Clear();
                CleanDays();
            });

            WeakReferenceMessenger.Default.Register<PhaseStoreMsgs.BetweenPointChangedMsg>(this, (r, m) =>
            {
                ReloadSchedule();
            });
            #endregion

            #region Commands
            ResetBetweenPointCommand = new(() => _phaseStore.ResetBetweenPoint());
            #endregion

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
        }

        #region Performance super speed code
        private void RemoveClassGroup(ClassGroupModel classGroupModel)
        {
            /// Check Map
            /// Tồn tại -> Lấy vị trí map -> Xoá
            /// Loại bỏ vị trí khỏi map
            /// Loại bỏ khỏi danh sách quản lý
            if (_timeBlockMap.IsExist(classGroupModel))
            {
                string id = classGroupModel.SubjectCode;
                Location location = _timeBlockMap.Map[id];
                RemoveScheduleItem(id, location);
                _timeBlockMap.Map.Remove(id);
                for (int i = 0; i < _classGroupModels.Count; i++)
                {
                    if (_classGroupModels[i].SubjectCode.Equals(id))
                    {
                        _classGroupModels.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// * Hight performance codes
        /// 
        /// RemoveScheduleItem
        /// </summary>
        /// <param name="location"></param>
        private void RemoveScheduleItem(string id, Location location)
        {
            if (location.Phase1.Any())
            {
                foreach (DayOfWeek dayOfWeek in location.Phase1)
                {
                    RemoveScheduleItemFromDay(id, Week1[dayOfWeek.ToIndex()]);
                }
            }

            if (location.Phase2.Any())
            {
                foreach (DayOfWeek dayOfWeek in location.Phase2)
                {
                    RemoveScheduleItemFromDay(id, Week2[dayOfWeek.ToIndex()]);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">
        /// ScheduleItem ID
        /// </param>
        /// <param name="day">
        /// Day of week
        /// </param>
        private static void RemoveScheduleItemFromDay(string id, ObservableCollection<TimeBlock> day)
        {
            if (day.Count == 0) return;
            int currentIndex = 0;
            bool isEndOfList = false;
            while (!isEndOfList)
            {
                if (day[currentIndex].ScheduleTableItemType == ScheduleTableItemType.SchoolClass
                    && day[currentIndex].Id.Equals(id))
                {
                    day.RemoveAt(currentIndex);
                }
                isEndOfList = currentIndex >= day.Count - 1;
                currentIndex++;
            }
        }

        /// <summary>
        /// * Hight performance codes
        /// 
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
        #endregion

        private void DivideSchoolClassesByPhases()
        {
            foreach (ClassGroupModel classGroupModel in _classGroupModels)
            {
                _timeBlockMap.AddScheduleItem(classGroupModel);
                AddClassGroup(classGroupModel);
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

        private void ReloadSchedule()
        {
            _timeBlockMap.Map.Clear();
            CleanDays();
            DivideSchoolClassesByPhases();
            DividePlaceConflictByPhase();
            DivideConflictByPhase();
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
