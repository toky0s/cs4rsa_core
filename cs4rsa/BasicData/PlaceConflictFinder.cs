using System;
using System.Collections.Generic;
using System.Linq;

namespace cs4rsa.BasicData
{

    /// <summary>
    /// Trình tìm kiếm xung đột hai giờ đầu và hai giờ cuối của hai ClassGroup
    /// </summary>
    class PlaceConflictFinder
    {
        private static readonly TimeSpan _timeDelta = new TimeSpan(0, 15, 0); // 15 minutes

        private readonly ClassGroup _classGroup1;
        private readonly ClassGroup _classGroup2;

        public PlaceConflictFinder(ClassGroup classGroup1, ClassGroup classGroup2)
        {
            _classGroup1 = classGroup1;
            _classGroup2 = classGroup2;
        }

        public PlaceConflict GetPlaceConflict()
        {
            DayPlaceMetaData dayPlaceMetaData1 = _classGroup1.GetDayPlaceMetaData();
            DayPlaceMetaData dayPlaceMetaData2 = _classGroup2.GetDayPlaceMetaData();
            List<DayOfWeek> studyDays = new List<DayOfWeek>();
            studyDays.AddRange(_classGroup1.GetDayOfWeeks());
            studyDays.AddRange(_classGroup2.GetDayOfWeeks());
            studyDays = studyDays.Distinct().ToList();

            if (Conflict.CanConflictPhase(_classGroup1.GetPhase(), _classGroup2.GetPhase()))
            {
                Schedule scheduleClassGroup1 = _classGroup1.GetSchedule();
                Schedule scheduleClassGroup2 = _classGroup2.GetSchedule();
                List<DayOfWeek> DayOfWeeks = ScheduleManipulation.GetIntersectDate(scheduleClassGroup1, scheduleClassGroup2);
                // Check date
                if (DayOfWeeks.Count > 0)
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
                        return new PlaceConflict(conflictPlaces);
                    return null;
                }
                return null;
            }
            return null;
        }
    }
}
