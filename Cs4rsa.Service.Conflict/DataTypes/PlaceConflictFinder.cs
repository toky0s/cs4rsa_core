using Cs4rsa.Service.Conflict.Models;
using Cs4rsa.Service.SubjectCrawler.DataTypes;
using Cs4rsa.Service.SubjectCrawler.DataTypes.Enums;
using Cs4rsa.Services.ConflictSvc.Utils;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Cs4rsa.Service.Conflict.DataTypes
{

    /// <summary>
    /// Trình tìm kiếm xung đột hai giờ đầu và hai giờ cuối của hai ClassGroup
    /// </summary>
    public class PlaceConflictFinder : BaseConflict
    {
        private static readonly TimeSpan _timeDelta = new TimeSpan(0, 15, 0); // 15 minutes

        public PlaceConflictFinder(Lesson lessonA, Lesson lessonB) : base(lessonA, lessonB)
        {
        }

        public ConflictPlace GetPlaceConflict()
        {
            // Hai school class không có giao nhau về giai đoạn chắc chắn không xung đột.
            // Check phase
            PhaseIntersect phaseIntersect = PhaseManipulation.GetPhaseIntersect(LessonA.StudyWeek, LessonB.StudyWeek);
            if (phaseIntersect.Equals(PhaseIntersect.NullInstance))
            {
                return null;
            }

            // Kiểm tra hai school class có ngày học chung hay không, nếu không
            // chắc chắn không xảy ra xung đột.
            Schedule scheduleA = LessonA.Schedule;
            Schedule scheduleB = LessonB.Schedule;
            IEnumerable<DayOfWeek> intersectDayOfWeeks = ScheduleManipulation.GetIntersectDate(scheduleA, scheduleB);
            if (!intersectDayOfWeeks.Any())
            {
                return null;
            }

            // Kiểm tra hai school class có cùng một nơi học hay không. Nếu cùng thì chắc chắn không
            // có xung đột. Nhưng nếu lớn hơn 1 nơi thì có khả năng gây ra xung đột.
            IEnumerable<Place> schoolClass1Places = LessonA.MetaData.GetPlaces();
            IEnumerable<Place> schoolClass2Places = LessonB.MetaData.GetPlaces();
            IEnumerable<Place> dinstictPlaces = schoolClass1Places.Concat(schoolClass2Places).Distinct();
            if (dinstictPlaces.Count() < 2)
            {
                return null;
            }

            var conflictPlaces = new Dictionary<DayOfWeek, IEnumerable<PlaceAdjacent>>();
            // Duyệt qua các thứ học để lấy ra các nơi học. Mỗi nơi học
            foreach (DayOfWeek dayOfWeek in intersectDayOfWeeks)
            {
                IEnumerable<PlaceMap> placeMap1 = LessonA.Cs4rsaMetaData.GetPlaceMapsAtDay(dayOfWeek);
                IEnumerable<PlaceMap> placeMap2 = LessonB.Cs4rsaMetaData.GetPlaceMapsAtDay(dayOfWeek);
                List<PlaceMap> placeMapsJoin = placeMap1.Concat(placeMap2).ToList();

                IEnumerable<Tuple<PlaceMap, PlaceMap>> placeMapPairs = PlaceMapManipulation.PairPlaceMaps(placeMapsJoin);
                IEnumerable<PlaceAdjacent> placeAdjacents = PlaceMapManipulation.GetPlaceAdjacents(
                    placeMapPairs,
                    _timeDelta
                );
                if (placeAdjacents.Any())
                    conflictPlaces.Add(dayOfWeek, placeAdjacents);
            }
            if (conflictPlaces.Count != 0)
            {
                return new ConflictPlace(conflictPlaces);
            }
            else
            {
                return null;
            }
        }
    }
}
