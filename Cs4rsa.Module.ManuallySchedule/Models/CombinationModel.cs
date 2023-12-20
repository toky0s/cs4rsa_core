using Cs4rsa.Service.Conflict.DataTypes;
using Cs4rsa.Service.Conflict.Models;
using Cs4rsa.Service.SubjectCrawler.DataTypes;
using Cs4rsa.UI.ScheduleTable.Models;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Cs4rsa.Module.ManuallySchedule.ViewModelFunctions;

namespace Cs4rsa.Module.ManuallySchedule.Models
{
    /// <summary>
    /// Class này đại diện cho sự kết hợp một tập các ClassGroupModel khác nhau.
    /// </summary>
    public class CombinationModel
    {
        public IEnumerable<SubjectModel> SubjectModels { get; set; }

        public IEnumerable<ClassGroupModel> ClassGroupModels { get; set; }

        public bool HaveAClassGroupHaveNotSchedule { get; set; }

        public bool HaveAClassGroupHaveZeroEmptySeat { get; set; }

        public ObservableCollection<ConflictModel> ConflictModels { get; set; }

        public ObservableCollection<PlaceConflictFinderModel> PlaceConflictFinderModels { get; set; }

        public bool IsCanShow { get; set; }
        public bool IsConflict { get; set; }

        public CombinationModel(
            IEnumerable<SubjectModel> subjectModels,
            IEnumerable<ClassGroupModel> classGroupModels
        )
        {
            ConflictModels = new ObservableCollection<ConflictModel>();
            PlaceConflictFinderModels = new ObservableCollection<PlaceConflictFinderModel>();
            SubjectModels = subjectModels;
            ClassGroupModels = classGroupModels;
            IsCanShow = !IsHaveAClassGroupHaveNotSchedule() && !IsHaveAClassGroupHaveZeroEmptySeat();
            UpdateConflict.UpdateConflictModelCollection(ConflictModels, classGroupModels);
            UpdateConflict.UpdatePlaceConflictCollection(PlaceConflictFinderModels, ClassGroupModels);
            IsConflict = ConflictModels.Count > 0 || PlaceConflictFinderModels.Count > 0;
        }

        private bool IsHaveAClassGroupHaveZeroEmptySeat()
        {
            return ClassGroupModels.Any(classGroupModel => classGroupModel.EmptySeat == 0);
        }

        /// <summary>
        /// Kiểm tra xem combination này có chứa một class group mà class group đó không có schedule
        /// hay không. Nếu không có trả về true,ngược lại trả về false.
        /// </summary>
        private bool IsHaveAClassGroupHaveNotSchedule()
        {
            foreach (var classGroupModel in ClassGroupModels)
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
            var count = 0;
            var subjectCode = string.Empty;
            foreach (var classGroupModel in ClassGroupModels)
            {
                if (classGroupModel.SubjectCode.Equals(subjectCode, System.StringComparison.Ordinal)) continue;
                subjectCode = classGroupModel.SubjectCode;
                count++;
            }
            return count == ClassGroupModels.Count();
        }

        /// <summary>
        /// Kiểm tra xem bộ này có chứa xung đột về thời gian hay không.
        /// </summary>
        public bool IsHaveTimeConflicts()
        {
            var schoolClasseModels = new List<SchoolClassModel>();
            foreach (var classGroupModel in ClassGroupModels)
            {
                schoolClasseModels.AddRange(classGroupModel.NormalSchoolClassModels);
            }
            for (var i = 0; i < schoolClasseModels.Count; ++i)
            {
                for (var k = i + 1; k < schoolClasseModels.Count; ++k)
                {
                    var lessonA = new Lesson(
                        schoolClasseModels[i].StudyWeek,
                        schoolClasseModels[i].Schedule,
                        schoolClasseModels[i].DayPlaceMetaData,
                        schoolClasseModels[i].SchoolClass.GetMetaDataMap(),
                        schoolClasseModels[i].Phase,
                        schoolClasseModels[i].SchoolClassName,
                        schoolClasseModels[i].SchoolClass.ClassGroupName,
                        schoolClasseModels[i].SubjectCode
                    );

                    var lessonB = new Lesson(
                        schoolClasseModels[k].StudyWeek,
                        schoolClasseModels[k].Schedule,
                        schoolClasseModels[k].DayPlaceMetaData,
                        schoolClasseModels[k].SchoolClass.GetMetaDataMap(),
                        schoolClasseModels[k].Phase,
                        schoolClasseModels[k].SchoolClassName,
                        schoolClasseModels[k].SchoolClass.ClassGroupName,
                        schoolClasseModels[k].SubjectCode
                    );

                    var conflict = new Conflict(lessonA, lessonB);
                    var conflictTime = conflict.GetConflictTime();
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
            var schoolClasses = new List<SchoolClassModel>();
            foreach (var classGroupModel in ClassGroupModels)
            {
                schoolClasses.AddRange(classGroupModel.NormalSchoolClassModels);
            }
            for (var i = 0; i < schoolClasses.Count; ++i)
            {
                for (var k = i + 1; k < schoolClasses.Count; ++k)
                {
                    var lessonA = new Lesson(
                        schoolClasses[i].StudyWeek, 
                        schoolClasses[i].Schedule, 
                        schoolClasses[i].DayPlaceMetaData, 
                        schoolClasses[i].SchoolClass.GetMetaDataMap(), 
                        schoolClasses[i].Phase, 
                        schoolClasses[i].SchoolClassName, 
                        schoolClasses[i].SchoolClass.ClassGroupName,
                        schoolClasses[i].SubjectCode
                    );

                    var lessonB = new Lesson(
                        schoolClasses[k].StudyWeek, 
                        schoolClasses[k].Schedule, 
                        schoolClasses[k].DayPlaceMetaData, 
                        schoolClasses[k].SchoolClass.GetMetaDataMap(), 
                        schoolClasses[k].Phase, 
                        schoolClasses[k].SchoolClassName, 
                        schoolClasses[k].SchoolClass.ClassGroupName,
                        schoolClasses[k].SubjectCode
                    );

                    var conflict = new PlaceConflictFinder(lessonA, lessonB);
                    var conflictPlace = conflict.GetPlaceConflict();
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
            var schedules = ClassGroupModels.Select(item => item.ClassGroup.GetSchedule());
            return ScheduleManipulation.MergeSchedule(schedules);
        }
    }
}
