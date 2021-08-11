using cs4rsa.BaseClasses;
using cs4rsa.Models;
using cs4rsa.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cs4rsa.BasicData
{

    /// <summary>
    /// Trình tìm kiếm xung đột hai giờ đầu và hai giờ cuối của hai ClassGroup
    /// </summary>
    public class PlaceConflictFinder : BaseConflict
    {
        private static readonly TimeSpan _timeDelta = new TimeSpan(0, 15, 0); // 15 minutes

        public PlaceConflictFinder(SchoolClass schoolClass1, SchoolClass schoolClass2) : base(schoolClass1, schoolClass2)
        {
        }

        public PlaceConflictFinder(SchoolClassModel schoolClass1, SchoolClassModel schoolClass2) : base(schoolClass1, schoolClass2)
        {
        }

        public ConflictPlace GetPlaceConflict()
        {
            // Hai school class không có giao nhau về giai đoạn chắc chắn không xung đột.
            if (CanConflictPhase(_schoolClass1.GetPhase(), _schoolClass2.GetPhase()))
            {
                // Kiểm tra hai school class có ngày học chung hay không, nếu không
                // chắc chắn không xảy ra xung đột.
                Schedule scheduleClassGroup1 = _schoolClass1.Schedule;
                Schedule scheduleClassGroup2 = _schoolClass2.Schedule;
                List<DayOfWeek> intersectDayOfWeeks = ScheduleManipulation.GetIntersectDate(scheduleClassGroup1, scheduleClassGroup2);
                if (intersectDayOfWeeks.Count > 0)
                {
                    // Kiểm tra hai school class có cùng một nơi học hay không. Nếu cùng thì chắc chắn không
                    // có xung đột. Nhưng nếu lớn hơn 1 nơi thì có khả năng gây ra xung đột.
                    List<Place> schoolClass1Places = _schoolClass1.DayPlaceMetaData.GetPlaces();
                    List<Place> schoolClass2Places = _schoolClass2.DayPlaceMetaData.GetPlaces();
                    List<Place> dinstictPlaces = schoolClass1Places.Concat(schoolClass2Places).Distinct().ToList();
                    if (dinstictPlaces.Count > 1)
                    {
                        Dictionary<DayOfWeek, List<PlaceAdjacent>> conflictPlaces = new Dictionary<DayOfWeek, List<PlaceAdjacent>>();
                        // Duyệt qua các thứ học để lấy ra các nơi học. Mỗi nơi học
                        foreach (DayOfWeek dayOfWeek in intersectDayOfWeeks)
                        {
                            List<PlaceMap> placeMap1 = _schoolClass1.GetMetaDataMap().GetPlaceMapsAtDay(dayOfWeek);
                            List<PlaceMap> placeMap2 = _schoolClass2.GetMetaDataMap().GetPlaceMapsAtDay(dayOfWeek);
                            List<PlaceMap> placeMapsJoin = placeMap1.Concat(placeMap2).ToList();

                            List<Tuple<PlaceMap, PlaceMap>> placeMapPairs = PlaceMapManipulation.PairPlaceMaps(placeMapsJoin);
                            List<PlaceAdjacent> placeAdjacents = PlaceMapManipulation.GetPlaceAdjacents(placeMapPairs, _timeDelta);
                            if (placeAdjacents.Count > 0)
                                conflictPlaces.Add(dayOfWeek, placeAdjacents);
                        }
                        if (conflictPlaces.Count != 0)
                            return new ConflictPlace(conflictPlaces);
                        return null;
                    }
                    return null;
                }
                return null;
            }
            return null;
        }

        public ConflictType GetConflictType()
        {
            return ConflictType.Place;
        }
    }
}
