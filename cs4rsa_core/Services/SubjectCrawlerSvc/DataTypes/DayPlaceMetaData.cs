using cs4rsa_core.Services.SubjectCrawlerSvc.DataTypes.Enums;

using System;
using System.Collections.Generic;
using System.Linq;

namespace cs4rsa_core.Services.SubjectCrawlerSvc.DataTypes
{
    /// <summary>
    /// Theo thiết kế, một SchoolClass sẽ có thể có nhiều tiết học
    /// tương ứng với đó là SchoolClassUnit.
    /// </summary>
    public class DayPlaceMetaData
    {
        private readonly Dictionary<DayOfWeek, DayRoomPlace> _dayPlacePairs;

        public DayPlaceMetaData()
        {
            _dayPlacePairs = new();
        }

        public void AddDayTimePair(DayOfWeek day, DayRoomPlace dayPlacePair)
        {
            _dayPlacePairs.Add(day, dayPlacePair);
        }

        public Place GetPlaceAtDay(DayOfWeek day)
        {
            return _dayPlacePairs[day].Place;
        }

        public DayRoomPlace GetDayPlacePairAtDay(DayOfWeek day)
        {
            return _dayPlacePairs[day];
        }

        public IEnumerable<Place> GetPlaces()
        {
            List<Place> places = new();
            foreach (DayRoomPlace pair in _dayPlacePairs.Values)
            {
                places.Add(pair.Place);
            }
            return places;
        }
    }
}
