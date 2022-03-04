using SubjectCrawlService1.DataTypes.Enums;
using System;
using System.Collections.Generic;

namespace SubjectCrawlService1.DataTypes
{
    /// <summary>
    /// Một Mapping giữa Schedule và DayPlaceMetaData
    /// cho phép lấy ra được vị trí học của các StudyTime trong một DayOfWeek nào đó.
    /// </summary>
    public class Cs4rsaMetaData
    {
        private readonly Schedule _schedule;
        private readonly DayPlaceMetaData _dayPlaceMetaData;
        public Cs4rsaMetaData(Schedule schedule, DayPlaceMetaData dayPlaceMetaData)
        {
            _schedule = schedule;
            _dayPlaceMetaData = dayPlaceMetaData;
        }

        public IEnumerable<PlaceMap> GetPlaceMapsAtDay(DayOfWeek dayOfWeek)
        {
            IEnumerable<StudyTime> studyTimes = _schedule.GetStudyTimesAtDay(dayOfWeek);
            Place place = _dayPlaceMetaData.GetPlaceAtDay(dayOfWeek);
            if (studyTimes != null)
            {
                foreach (StudyTime studyTime in studyTimes)
                {
                    yield return new PlaceMap(studyTime, place);
                }
            }
            yield break;
        }
    }
}
