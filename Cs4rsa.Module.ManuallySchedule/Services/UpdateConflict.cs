using Cs4rsa.Module.ManuallySchedule.Models;
using Cs4rsa.Service.Conflict.DataTypes;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Cs4rsa.Service.Conflict.Models;

namespace Cs4rsa.Module.ManuallySchedule.Services
{
    /// <summary>
    /// Mục đích của class này được tạo ra là để các phần xếp lịch thủ công và xếp lịch tự động
    /// được sử dụng chung với nhau việc cập nhật các xung đột về thời gian và vị trí học.
    /// </summary>
    public static class UpdateConflict
    {
        /// <summary>
        /// Thực hiện bắt cặp tất cả các ClassGroupModel có 
        /// trong Collection để phát hiện các Conflict Time.
        /// </summary>
        public static void UpdateConflictModelCollection(
            ObservableCollection<Conflict> conflictModels,
            IEnumerable<ClassGroupModel> classGroupModels)
        {
            conflictModels.Clear();
            var schoolClasses = new List<SchoolClassModel>();
            foreach (var classGroupModel in classGroupModels)
            {
                schoolClasses.AddRange(classGroupModel.NormalSchoolClassModels);
            }

            for (var i = 0; i < schoolClasses.Count; ++i)
            {
                for (var k = i + 1; k < schoolClasses.Count; ++k)
                {
                    if (schoolClasses[i].SchoolClass.ClassGroupName.Equals(schoolClasses[k].SchoolClass.ClassGroupName))
                        continue;
                    
                    var lessonA = new Lesson(
                        schoolClasses[i].StudyWeek,
                        schoolClasses[i].Schedule,
                        schoolClasses[i].DayPlaceMetaData,
                        schoolClasses[i].SchoolClass.Metadata,
                        schoolClasses[i].Phase,
                        schoolClasses[i].SchoolClassName,
                        schoolClasses[i].SchoolClass.ClassGroupName,
                        schoolClasses[i].SubjectCode
                    );
                    
                    var lessonB = new Lesson(
                        schoolClasses[k].StudyWeek,
                        schoolClasses[k].Schedule,
                        schoolClasses[k].DayPlaceMetaData,
                        schoolClasses[k].SchoolClass.Metadata,
                        schoolClasses[k].Phase,
                        schoolClasses[k].SchoolClassName,
                        schoolClasses[k].SchoolClass.ClassGroupName,
                        schoolClasses[k].SubjectCode
                    );
                    
                    var conflict = new Conflict(lessonA, lessonB);
                    var conflictTime = conflict.ConflictTime;
                    if (conflictTime == null) continue;
                    conflictModels.Add(conflict);
                }
            }
        }

        public static void UpdatePlaceConflictCollection(
            ObservableCollection<PlaceConflict> placeConflictFinderModels
            , IEnumerable<ClassGroupModel> classGroupModels)
        {
            placeConflictFinderModels.Clear();
            var schoolClasses = new List<SchoolClassModel>();
            foreach (var classGroupModel in classGroupModels)
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
                        schoolClasses[i].SchoolClass.Metadata,
                        schoolClasses[i].Phase,
                        schoolClasses[i].SchoolClassName,
                        schoolClasses[i].SchoolClass.ClassGroupName,
                        schoolClasses[i].SubjectCode
                    );
                    
                    var lessonB = new Lesson(
                        schoolClasses[k].StudyWeek,
                        schoolClasses[k].Schedule,
                        schoolClasses[k].DayPlaceMetaData,
                        schoolClasses[k].SchoolClass.Metadata,
                        schoolClasses[k].Phase,
                        schoolClasses[k].SchoolClassName,
                        schoolClasses[k].SchoolClass.ClassGroupName,
                        schoolClasses[k].SubjectCode
                    );
                    
                    var placeConflict = new PlaceConflict(lessonA, lessonB);
                    var conflictPlace = placeConflict.ConflictPlace;
                    if (conflictPlace == null) continue;
                    placeConflictFinderModels.Add(placeConflict);
                }
            }
        }
    }
}
