using System;
using System.Collections.Generic;

namespace Cs4rsa.Service.SubjectCrawler.DataTypes
{
    /// <summary>
    /// Một Mapping giữa Schedule và DayPlaceMetaData
    /// cho phép lấy ra được vị trí học của các StudyTime trong một DayOfWeek nào đó.
    /// </summary>
    public class Cs4rsaMetaData
    {
        private readonly Schedule _schedule;
        private readonly DayPlaceMetaData _dayPlaceMetaData;
        private readonly SchoolClass _schoolClass;

        public Cs4rsaMetaData(
            Schedule schedule,
            DayPlaceMetaData dayPlaceMetaData,
            SchoolClass schoolClass)
        {
            _schedule = schedule;
            _dayPlaceMetaData = dayPlaceMetaData;
            _schoolClass = schoolClass;
        }

        public IEnumerable<PlaceMap> GetPlaceMapsAtDay(DayOfWeek dayOfWeek)
        {
            var studyTimes = _schedule.GetStudyTimesAtDay(dayOfWeek);
            var place = _dayPlaceMetaData.GetPlaceAtDay(dayOfWeek);
            if (studyTimes == null) yield break;
            foreach (var studyTime in studyTimes)
            {
                yield return new PlaceMap(studyTime, place, _schoolClass);
            }
        }
    }
}
