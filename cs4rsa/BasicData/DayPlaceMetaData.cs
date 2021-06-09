using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.BasicData
{
    /// <summary>
    /// Đại diện cho một phòng học.
    /// </summary>
    public class Room
    {
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            private set
            {
                _name = value;
            }
        }

        private Place _place;
        public Place Place
        {
            get
            {
                return _place;
            }
            private set
            {
                _place = value;
            }
        }

        public Room(string name)
        {
            _name = name;
        }

        public Room(string name, Place place)
        {
            _name = name;
            _place = place;
        }
    }

    public class DayPlacePair
    {
        private DayOfWeek _day;
        private Room _room;
        private Place _place;

        public DayOfWeek Day
        {
            get
            {
                return _day;
            }
            private set
            {
                _day = value;
            }
        }
        public Room Room
        {
            get
            {
                return _room;
            }
            private set
            {
                _room = value;
            }
        }
        public Place Place
        {
            get
            {
                return _place;
            }
            private set
            {
                _place = value;
            }
        }

        public DayPlacePair(DayOfWeek day, Room room, Place place)
        {
            _day = day;
            _room = room;
            _place = place;
        }
    }

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

        public Place GetPlaceAddDay(DayOfWeek day)
        {
            return _dayPlacePairs[day].Place;
        }
    }
}
