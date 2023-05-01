using Cs4rsa.Services.ConflictSvc.DataTypes;
using Cs4rsa.Services.ConflictSvc.Models;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes;
using Cs4rsa.Services.SubjectCrawlerSvc.Models;
using Cs4rsa.ViewModelFunctions;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Cs4rsa.Models
{
    /// <summary>
    /// Class này đại diện cho sự kết hợp một tập các ClassGroupModel khác nhau.
    /// </summary>
    public class CombinationModel
    {
        public IEnumerable<SubjectModel> SubjecModels { get; set; }

        private IEnumerable<ClassGroupModel> _classGroupModels;
        public IEnumerable<ClassGroupModel> ClassGroupModels
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

        public CombinationModel(
            IEnumerable<SubjectModel> subjectModels,
            IEnumerable<ClassGroupModel> classGroupModels
        )
        {
            SubjecModels = subjectModels;
            _classGroupModels = classGroupModels;
            IsCanShow = !IsHaveAClassGroupHaveNotSchedule() && !IsHaveAClassGroupHaveZeroEmptySeat();
            UpdateConflict.UpdateConflictModelCollection(_conflictModels, _classGroupModels);
            UpdateConflict.UpdatePlaceConflictCollection(_placeConflictFinderModels, _classGroupModels);
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
        public bool IsValid()
        {
            int count = 0;
            string subjecCode = string.Empty;
            foreach (ClassGroupModel classGroupModel in _classGroupModels)
            {
                if (!classGroupModel.SubjectCode.Equals(subjecCode, System.StringComparison.Ordinal))
                {
                    subjecCode = classGroupModel.SubjectCode;
                    count++;
                }
            }
            return count == _classGroupModels.Count();
        }

        /// <summary>
        /// Kiểm tra xem bộ này có chứa xung đột về thời gian hay không.
        /// </summary>
        public bool IsHaveTimeConflicts()
        {
            List<SchoolClassModel> schoolClasseModels = new();
            foreach (ClassGroupModel classGroupModel in _classGroupModels)
            {
                schoolClasseModels.AddRange(classGroupModel.NormalSchoolClassModels);
            }
            for (int i = 0; i < schoolClasseModels.Count; ++i)
            {
                for (int k = i + 1; k < schoolClasseModels.Count; ++k)
                {
                    Conflict conflict = new(schoolClasseModels[i], schoolClasseModels[k]);
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
            List<SchoolClassModel> schoolClasses = new();
            foreach (ClassGroupModel classGroupModel in _classGroupModels)
            {
                schoolClasses.AddRange(classGroupModel.NormalSchoolClassModels);
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
            IEnumerable<Schedule> schedules = _classGroupModels.Select(item => item.ClassGroup.GetSchedule());
            return ScheduleManipulation.MergeSchedule(schedules);
        }
    }
}
