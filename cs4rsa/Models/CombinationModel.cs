using cs4rsa.BasicData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.Models
{
    /// <summary>
    /// Class này đại diện cho sự kết hợp một tập các ClassGroupModel khác nhau.
    /// </summary>
    public class CombinationModel
    {
        private List<SubjectModel> _subjectModels;
        public List<SubjectModel> SubjecModels
        {
            get
            {
                return _subjectModels;
            }
            set
            {
                _subjectModels = value;
            }
        }

        private List<ClassGroupModel> _classGroupModels;
        public List<ClassGroupModel> ClassGroupModels
        {
            get
            {
                return _classGroupModels;
            }
            set
            {
                _classGroupModels = value;
            }
        }

        private bool _haveAClassGroupHaveNotSchedule;
        public bool HaveAClassGroupHaveNotSchedule
        {
            get { return _haveAClassGroupHaveNotSchedule; }
            set { _haveAClassGroupHaveNotSchedule = value; }
        }

        private bool _haveAClassGroupHaveZeroEmptySeat;
        public bool HaveAClassGroupHaveZeroEmptySeat
        {
            get { return _haveAClassGroupHaveZeroEmptySeat; }
            set { _haveAClassGroupHaveZeroEmptySeat = value; }
        }

        private ObservableCollection<ConflictModel> _conflictModels = new ObservableCollection<ConflictModel>();
        public ObservableCollection<ConflictModel> ConflictModels
        {
            get { return _conflictModels; }
            set { _conflictModels = value; }
        }

        private ObservableCollection<PlaceConflictFinderModel> _placeConflictFinderModels = new ObservableCollection<PlaceConflictFinderModel>();
        public ObservableCollection<PlaceConflictFinderModel> PlaceConflictFinderModels
        {
            get { return _placeConflictFinderModels; }
            set { _placeConflictFinderModels = value; }
        }

        private bool _canShow;
        public bool CanShow
        {
            get { return _canShow; }
            set { _canShow = value; }
        }

        public CombinationModel(List<SubjectModel> subjectModels, List<ClassGroupModel> classGroupModels)
        {
            _subjectModels = subjectModels;
            _classGroupModels = classGroupModels;
            _haveAClassGroupHaveNotSchedule = IsHaveAClassGroupHaveNotSchedule();
            _haveAClassGroupHaveZeroEmptySeat = IsHaveAClassGroupHaveZeroEmptySeat();
            _canShow = !_haveAClassGroupHaveZeroEmptySeat && !_haveAClassGroupHaveNotSchedule;
            UpdateConflictModels();
            UpdatePlaceConflictModels();
        }

        private void UpdatePlaceConflictModels()
        {
            _placeConflictFinderModels.Clear();
            List<SchoolClass> schoolClasses = new List<SchoolClass>();
            foreach (ClassGroupModel classGroupModel in _classGroupModels)
            {
                schoolClasses.AddRange(classGroupModel.ClassGroup.SchoolClasses);
            }
            for (int i = 0; i < schoolClasses.Count; ++i)
            {
                for (int k = i + 1; k < schoolClasses.Count; ++k)
                {
                    PlaceConflictFinder placeConflict = new PlaceConflictFinder(schoolClasses[i], schoolClasses[k]);
                    ConflictPlace conflictPlace = placeConflict.GetPlaceConflict();
                    if (conflictPlace != null)
                    {
                        PlaceConflictFinderModel placeConflictModel = new PlaceConflictFinderModel(placeConflict);
                        _placeConflictFinderModels.Add(placeConflictModel);
                    }
                }
            }
        }

        private void UpdateConflictModels()
        {
            _conflictModels.Clear();
            List<SchoolClass> schoolClasses = new List<SchoolClass>();
            foreach (ClassGroupModel classGroupModel in _classGroupModels)
            {
                schoolClasses.AddRange(classGroupModel.ClassGroup.SchoolClasses);
            }
            for (int i = 0; i < _classGroupModels.Count; ++i)
            {
                for (int k = i + 1; k < _classGroupModels.Count; ++k)
                {
                    Conflict conflict = new Conflict(schoolClasses[i], schoolClasses[k]);
                    ConflictTime conflictTime = conflict.GetConflictTime();
                    if (conflictTime != null)
                    {
                        ConflictModel conflictModel = new ConflictModel(conflict);
                        _conflictModels.Add(conflictModel);
                    }
                }
            }
        }

        private bool IsHaveAClassGroupHaveZeroEmptySeat()
        {
            foreach (ClassGroupModel classGroupModel in _classGroupModels)
            {
                if (classGroupModel.EmptySeat == 0)
                    return true;
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
            foreach(ClassGroupModel classGroupModel in _classGroupModels)
            {
                if (!classGroupModel.HaveSchedule)
                    return true;
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
                if (!classGroupModel.SubjectCode.Equals(subjecCode))
                {
                    subjecCode = classGroupModel.SubjectCode;
                    count++;
                }
            }
            if (count == _classGroupModels.Count)
                return true;
            return false;
        }

        /// <summary>
        /// Kiểm tra xem bộ này có chứa xung đột về thời gian hay không.
        /// </summary>
        /// <returns></returns>
        public bool IsHaveTimeConflicts()
        {
            List<SchoolClass> schoolClasses = new List<SchoolClass>();
            foreach (ClassGroupModel classGroupModel in _classGroupModels)
            {
                schoolClasses.AddRange(classGroupModel.ClassGroup.SchoolClasses);
            }
            for (int i = 0; i < schoolClasses.Count; ++i)
            {
                for (int k = i + 1; k < schoolClasses.Count; ++k)
                {
                    Conflict conflict = new Conflict(schoolClasses[i], schoolClasses[k]);
                    ConflictTime conflictTime = conflict.GetConflictTime();
                    if (conflictTime != null)
                        return true;
                }
            }
            return false;
        }

        public bool IsHavePlaceConflicts()
        {
            List<SchoolClass> schoolClasses = new List<SchoolClass>();
            foreach (ClassGroupModel classGroupModel in _classGroupModels)
            {
                schoolClasses.AddRange(classGroupModel.ClassGroup.SchoolClasses);
            }
            for (int i = 0; i < schoolClasses.Count; ++i)
            {
                for (int k = i + 1; k < schoolClasses.Count; ++k)
                {
                    PlaceConflictFinder conflict = new PlaceConflictFinder(schoolClasses[i], schoolClasses[k]);
                    ConflictPlace conflictPlace = conflict.GetPlaceConflict();
                    if (conflictPlace != null)
                        return true;
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
