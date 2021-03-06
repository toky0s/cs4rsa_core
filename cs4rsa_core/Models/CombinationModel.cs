using ConflictService.DataTypes;
using ConflictService.Models;

using cs4rsa_core.ViewModelFunctions;

using SubjectCrawlService1.DataTypes;
using SubjectCrawlService1.Models;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace cs4rsa_core.Models
{
    /// <summary>
    /// Class này đại diện cho sự kết hợp một tập các ClassGroupModel khác nhau.
    /// </summary>
    public class CombinationModel
    {
        public List<SubjectModel> SubjecModels { get; set; }

        private List<ClassGroupModel> _classGroupModels;
        public List<ClassGroupModel> ClassGroupModels
        {
            get => _classGroupModels;
            set => _classGroupModels = value;
        }

        public bool HaveAClassGroupHaveNotSchedule { get; set; }

        public bool HaveAClassGroupHaveZeroEmptySeat { get; set; }

        private ObservableCollection<ConflictModel> _conflictModels = new();
        public ObservableCollection<ConflictModel> ConflictModels
        {
            get => _conflictModels;
            set => _conflictModels = value;
        }

        private ObservableCollection<PlaceConflictFinderModel> _placeConflictFinderModels = new();
        public ObservableCollection<PlaceConflictFinderModel> PlaceConflictFinderModels
        {
            get { return _placeConflictFinderModels; }
            set { _placeConflictFinderModels = value; }
        }

        public bool IsCanShow { get; set; }
        public bool IsConflict { get; set; }

        public CombinationModel(List<SubjectModel> subjectModels, List<ClassGroupModel> classGroupModels)
        {
            SubjecModels = subjectModels;
            _classGroupModels = classGroupModels;
            IsCanShow = !IsHaveAClassGroupHaveNotSchedule() && !IsHaveAClassGroupHaveZeroEmptySeat();
            UpdateConflict.UpdateConflictModelCollection(ref _conflictModels, ref _classGroupModels);
            UpdateConflict.UpdatePlaceConflictCollection(ref _placeConflictFinderModels, ref _classGroupModels);
            IsConflict = _conflictModels.Count > 0 || _placeConflictFinderModels.Count > 0;
        }

        private bool IsHaveAClassGroupHaveZeroEmptySeat()
        {
            foreach (ClassGroupModel classGroupModel in _classGroupModels)
            {
                if (classGroupModel.EmptySeat == 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Kiểm tra xem combination này có chứa một class group mà class group đó không có schedule
        /// hay không. Nếu không có trả về true,ngược lại trả về false.
        /// </summary>
        /// <returns></returns>
        private bool IsHaveAClassGroupHaveNotSchedule()
        {
            foreach (ClassGroupModel classGroupModel in _classGroupModels)
            {
                if (!classGroupModel.HaveSchedule)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Kiểm tra xem Bộ này có hợp lệ hay không. Một Bộ hợp lệ là khi từng ClassGroupModel
        /// bên trong thuộc một Subject, mà mỗi Subject là duy nhất.
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            int count = 0;
            string subjecCode = "";
            foreach (ClassGroupModel classGroupModel in _classGroupModels)
            {
                if (!classGroupModel.SubjectCode.Equals(subjecCode, System.StringComparison.Ordinal))
                {
                    subjecCode = classGroupModel.SubjectCode;
                    count++;
                }
            }
            return count == _classGroupModels.Count;
        }

        /// <summary>
        /// Kiểm tra xem bộ này có chứa xung đột về thời gian hay không.
        /// </summary>
        /// <returns></returns>
        public bool IsHaveTimeConflicts()
        {
            List<SchoolClass> schoolClasses = new();
            foreach (ClassGroupModel classGroupModel in _classGroupModels)
            {
                schoolClasses.AddRange(classGroupModel.ClassGroup.SchoolClasses);
            }
            for (int i = 0; i < schoolClasses.Count; ++i)
            {
                for (int k = i + 1; k < schoolClasses.Count; ++k)
                {
                    Conflict conflict = new(schoolClasses[i], schoolClasses[k]);
                    ConflictTime conflictTime = conflict.GetConflictTime();
                    if (conflictTime != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsHavePlaceConflicts()
        {
            List<SchoolClass> schoolClasses = new();
            foreach (ClassGroupModel classGroupModel in _classGroupModels)
            {
                schoolClasses.AddRange(classGroupModel.ClassGroup.SchoolClasses);
            }
            for (int i = 0; i < schoolClasses.Count; ++i)
            {
                for (int k = i + 1; k < schoolClasses.Count; ++k)
                {
                    PlaceConflictFinder conflict = new(schoolClasses[i], schoolClasses[k]);
                    ConflictPlace conflictPlace = conflict.GetPlaceConflict();
                    if (conflictPlace != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public Schedule GetSchedule()
        {
            List<Schedule> schedules = _classGroupModels.Select(item => item.ClassGroup.GetSchedule()).ToList();
            return ScheduleManipulation.MergeSchedule(schedules);
        }
    }
}
