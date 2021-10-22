using ConflictService.DataTypes;
using cs4rsa_core.Messages;
using cs4rsa_core.Models;
using LightMessageBus;
using SubjectCrawlService1.DataTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
            List<SchoolClass> schoolClasses = new List<SchoolClass>();
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
                    Conflict conflict = new Conflict(schoolClasses[i], schoolClasses[k]);
                    ConflictTime conflictTime = conflict.GetConflictTime();
                    if (conflictTime != null)
                    {
                        ConflictModel conflictModel = new ConflictModel(conflict);
                        conflictModels.Add(conflictModel);
                    }
                }
            }
        }

        public static void UpdatePlaceConflictCollection(ref ObservableCollection<PlaceConflictFinderModel> placeConflictFinderModels, ref List<ClassGroupModel> classGroupModels)
        {
            placeConflictFinderModels.Clear();
            List<SchoolClass> schoolClasses = new List<SchoolClass>();
            foreach (ClassGroupModel classGroupModel in classGroupModels)
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
                        placeConflictFinderModels.Add(placeConflictModel);
                    }
                }
            }
        }
    }
}
