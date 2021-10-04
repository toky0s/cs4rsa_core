using SubjectCrawlService1.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubjectCrawlService1.DataTypes
{
    /// <summary>
    /// Một Mapping giữa Schedule và DayPlaceMetaData
    /// cho phép lấy ra được vị trí học của các StudyTime trong một DayOfWeek nào đó.
    /// </summary>
    public class MetaDataMap
    {
        private Schedule _schedule;
        private DayPlaceMetaData _dayPlaceMetaData;
        public MetaDataMap(Schedule schedule, DayPlaceMetaData dayPlaceMetaData)
        {
            _schedule = schedule;
            _dayPlaceMetaData = dayPlaceMetaData;
        }

        public List<PlaceMap> GetPlaceMapsAtDay(DayOfWeek dayOfWeek)
        {
            List<StudyTime> studyTimes = _schedule.GetStudyTimesAtDay(dayOfWeek);
            Place place = _dayPlaceMetaData.GetPlaceAtDay(dayOfWeek);
            if (studyTimes != null)
            {
                List<PlaceMap> placeMaps = new List<PlaceMap>();
                foreach (StudyTime studyTime in studyTimes)
                {
                    placeMaps.Add(new PlaceMap(studyTime, place));
                }
                return placeMaps;
            }
            return null;
        }
    }
}
