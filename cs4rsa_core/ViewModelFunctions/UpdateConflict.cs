﻿using Cs4rsa.Services.ConflictSvc.DataTypes;
using Cs4rsa.Services.ConflictSvc.Models;
using Cs4rsa.Services.SubjectCrawlerSvc.Models;

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Cs4rsa.ViewModelFunctions
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
            ObservableCollection<ConflictModel> conflictModels,
            IEnumerable<ClassGroupModel> classGroupModels)
        {
            conflictModels.Clear();
            List<SchoolClassModel> schoolClasses = new();
            foreach (ClassGroupModel classGroupModel in classGroupModels)
            {
                schoolClasses.AddRange(classGroupModel.NormalSchoolClassModels);
            }

            for (int i = 0; i < schoolClasses.Count; ++i)
            {
                for (int k = i + 1; k < schoolClasses.Count; ++k)
                {
                    if (schoolClasses[i].SchoolClass.ClassGroupName.Equals(schoolClasses[k].SchoolClass.ClassGroupName))
                        continue;
                    Conflict conflict = new(schoolClasses[i], schoolClasses[k]);
                    ConflictTime conflictTime = conflict.GetConflictTime();
                    if (conflictTime == null) continue;
                    ConflictModel conflictModel = new(conflict);
                    conflictModels.Add(conflictModel);
                }
            }
        }

        public static void UpdatePlaceConflictCollection(
            ObservableCollection<PlaceConflictFinderModel> placeConflictFinderModels
            , IEnumerable<ClassGroupModel> classGroupModels)
        {
            placeConflictFinderModels.Clear();
            List<SchoolClassModel> schoolClasses = new();
            foreach (ClassGroupModel classGroupModel in classGroupModels)
            {
                schoolClasses.AddRange(classGroupModel.NormalSchoolClassModels);
            }

            for (int i = 0; i < schoolClasses.Count; ++i)
            {
                for (int k = i + 1; k < schoolClasses.Count; ++k)
                {
                    PlaceConflictFinder placeConflict = new(schoolClasses[i], schoolClasses[k]);
                    ConflictPlace conflictPlace = placeConflict.GetPlaceConflict();
                    if (conflictPlace == null) continue;
                    PlaceConflictFinderModel placeConflictModel = new(placeConflict);
                    placeConflictFinderModels.Add(placeConflictModel);
                }
            }
        }
    }
}
