using SubjectCrawlService1.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SubjectCrawlService1.DataTypes
{
    public class DayPlaceMetaData
    {
        private Dictionary<DayOfWeek, DayPlacePair> _dayPlacePairs = new Dictionary<DayOfWeek, DayPlacePair>();

        public DayPlaceMetaData()
        {

        }

        public DayPlaceMetaData(Dictionary<DayOfWeek, DayPlacePair> dayPlacePairs)
        {
            _dayPlacePairs = dayPlacePairs;
        }

        public void AddDayTimePair(DayOfWeek day, DayPlacePair dayPlacePair)
        {
            _dayPlacePairs.Add(day, dayPlacePair);
        }

        public Place GetPlaceAtDay(DayOfWeek day)
        {
            return _dayPlacePairs[day].Place;
        }

        public List<Place> GetPlaces()
        {
            List<Place> places = new List<Place>();
            foreach (DayPlacePair pair in _dayPlacePairs.Values)
            {
                places.Add(pair.Place);
            }
            return places.Distinct().ToList();
        }
    }
}
