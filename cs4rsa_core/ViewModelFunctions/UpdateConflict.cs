using ConflictService.DataTypes;
using ConflictService.Models;

using cs4rsa_core.Models;

using SubjectCrawlService1.DataTypes;
using SubjectCrawlService1.Models;

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace cs4rsa_core.ViewModelFunctions
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
        public static void UpdateConflictModelCollection(ref ObservableCollection<ConflictModel> conflictModels, ref List<ClassGroupModel> classGroupModels)
        {
            conflictModels.Clear();
            List<SchoolClass> schoolClasses = new();
            foreach (ClassGroupModel classGroupModel in classGroupModels)
            {
                schoolClasses.AddRange(classGroupModel.ClassGroup.SchoolClasses);
            }

            for (int i = 0; i < schoolClasses.Count; ++i)
            {
                for (int k = i + 1; k < schoolClasses.Count; ++k)
                {
                    if (schoolClasses[i].ClassGroupName.Equals(schoolClasses[k].ClassGroupName))
                        continue;
                    Conflict conflict = new(schoolClasses[i], schoolClasses[k]);
                    ConflictTime conflictTime = conflict.GetConflictTime();
                    if (conflictTime != null)
                    {
                        ConflictModel conflictModel = new(conflict);
                        conflictModels.Add(conflictModel);
                    }
                }
            }
        }

        public static void UpdatePlaceConflictCollection(ref ObservableCollection<PlaceConflictFinderModel> placeConflictFinderModels, ref List<ClassGroupModel> classGroupModels)
        {
            placeConflictFinderModels.Clear();
            List<SchoolClass> schoolClasses = new();
            foreach (ClassGroupModel classGroupModel in classGroupModels)
            {
                schoolClasses.AddRange(classGroupModel.ClassGroup.SchoolClasses);
            }

            for (int i = 0; i < schoolClasses.Count; ++i)
            {
                for (int k = i + 1; k < schoolClasses.Count; ++k)
                {
                    PlaceConflictFinder placeConflict = new(schoolClasses[i], schoolClasses[k]);
                    ConflictPlace conflictPlace = placeConflict.GetPlaceConflict();
                    if (conflictPlace != null)
                    {
                        PlaceConflictFinderModel placeConflictModel = new(placeConflict);
                        placeConflictFinderModels.Add(placeConflictModel);
                    }
                }
            }
        }
    }
}
