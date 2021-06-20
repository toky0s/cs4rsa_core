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
    class PlaceConflictFinder : BaseConflict
    {
        private static readonly TimeSpan _timeDelta = new TimeSpan(0, 15, 0); // 15 minutes

        public PlaceConflictFinder(ClassGroup classGroup1, ClassGroup classGroup2) : base(classGroup1, classGroup2)
        {
        }

        public PlaceConflictFinder(ClassGroupModel classGroup1, ClassGroupModel classGroup2)
            : base(classGroup1.classGroup, classGroup2.classGroup)
        {
        }

        public ConflictPlace GetPlaceConflict()
        {
            DayPlaceMetaData dayPlaceMetaData1 = _classGroup1.GetDayPlaceMetaData();
            DayPlaceMetaData dayPlaceMetaData2 = _classGroup2.GetDayPlaceMetaData();
            List<DayOfWeek> studyDays = new List<DayOfWeek>();
            studyDays.AddRange(_classGroup1.GetDayOfWeeks());
            studyDays.AddRange(_classGroup2.GetDayOfWeeks());
            studyDays = studyDays.Distinct().ToList();

            if (CanConflictPhase(_classGroup1.GetPhase(), _classGroup2.GetPhase()))
            {
                Schedule scheduleClassGroup1 = _classGroup1.GetSchedule();
                Schedule scheduleClassGroup2 = _classGroup2.GetSchedule();
                List<DayOfWeek> DayOfWeeks = ScheduleManipulation.GetIntersectDate(scheduleClassGroup1, scheduleClassGroup2);
                // Check date
                if (DayOfWeeks.Count > 0)
                {
                    List<Place> classGroup1Places = _classGroup1.GetDayPlaceMetaData().GetPlaces();
                    List<Place> classGroup2Places = _classGroup2.GetDayPlaceMetaData().GetPlaces();
                    List<Place> intersectedPlaces = classGroup1Places.Intersect(classGroup2Places).ToList();
                    if (intersectedPlaces.Count >= 2)
                    {
                        Dictionary<DayOfWeek, List<PlaceAdjacent>> conflictPlaces = new Dictionary<DayOfWeek, List<PlaceAdjacent>>();
                        //check time
                        foreach (DayOfWeek dayOfWeek in DayOfWeeks)
                        {
                            List<PlaceMap> placeMap1 = _classGroup1.GetMetaDataMap().GetPlaceMapsAtDay(dayOfWeek);
                            List<PlaceMap> placeMap2 = _classGroup2.GetMetaDataMap().GetPlaceMapsAtDay(dayOfWeek);
                            List<PlaceMap> placeMapsJoin = placeMap1.Concat(placeMap2).ToList();
                            List<Tuple<PlaceMap, PlaceMap>> placeMapPairs = PlaceMapManipulation.PairPlaceMaps(placeMapsJoin);
                            List<PlaceAdjacent> placeAdjacents = PlaceMapManipulation.GetPlaceAdjacents(placeMapPairs, _timeDelta);
                            if (placeMapPairs.Count > 0)
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
