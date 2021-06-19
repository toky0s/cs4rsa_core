using System;
using System.Collections.Generic;
using System.Linq;

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

        public Place GetPlaceAtDay(DayOfWeek day)
        {
            return _dayPlacePairs[day].Place;
        }
    }


    /// <summary>
    /// PlaceMap đại diện cho một khối thời gian đi kèm vị trí nơi học.
    /// </summary>
    public class PlaceMap: IComparable<PlaceMap>
    {
        public StudyTime StudyTime { get; set; }
        public Place Place { get; set; }

        public PlaceMap(StudyTime studyTime, Place place)
        {
            StudyTime = studyTime;
            Place = place;
        }

        public int CompareTo(PlaceMap other)
        {
            return StudyTime.Start.CompareTo(other.StudyTime.Start);
        }

        public override string ToString()
        {
            return $"{Place} {StudyTime}";
        }
    }

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

    public class PlaceMapManipulation
    {
        /// <summary>
        /// Bắt cặp các StudyTime trong cùng một List. 
        /// Giống với phương thức StudyTimeManipulation.PairStudyTimes()
        /// </summary>
        /// <param name="studyTimes">List các PlaceMap.</param>
        /// <returns>List các Tuple là cặp các PlaceMap.</returns>
        public static List<Tuple<PlaceMap, PlaceMap>> PairPlaceMaps(List<PlaceMap> placeMaps)
        {
            List<Tuple<PlaceMap, PlaceMap>> tuplePlaceMaps = new List<Tuple<PlaceMap, PlaceMap>>();
            int index = 0;
            while (index < placeMaps.Count() - 1)
            {
                PlaceMap firstItem = placeMaps[index];
                for (int j = index + 1; j <= placeMaps.Count() - 1; ++j)
                {
                    Tuple<PlaceMap, PlaceMap> tuplePlaceMap = new Tuple<PlaceMap, PlaceMap>(firstItem, placeMaps[j]);
                    tuplePlaceMaps.Add(tuplePlaceMap);
                }
                index++;
            }
            return tuplePlaceMaps;
        }


        /// <summary>
        /// Phát hiện xung đột Vị trí học.
        /// </summary>
        /// <param name="placeMapPairs"></param>
        /// <param name="timeDelta">TimeDelta đại diện cho một khoảng thời gian tối thiểu để
        /// chấp nhận một xung đột vị trí giữa hai nơi học.</param>
        /// <returns></returns>
        public static List<PlaceAdjacent> GetPlaceAdjacents(List<Tuple<PlaceMap, PlaceMap>> placeMapPairs, TimeSpan timeDelta)
        {
            List<PlaceAdjacent> placeAdjacents = new List<PlaceAdjacent>();
            foreach (Tuple<PlaceMap, PlaceMap> placeMapTuple in placeMapPairs)
            {
                PlaceAdjacent placeAdjacent = GetPlaceAdjacent(placeMapTuple.Item1, placeMapTuple.Item2, timeDelta);
                if (placeAdjacent!=null)
                    placeAdjacents.Add(placeAdjacent);
            }
            return placeAdjacents;
        }

        private static PlaceAdjacent GetPlaceAdjacent(PlaceMap placeMap1, PlaceMap placeMap2, TimeSpan timeDelta)
        {
            List<PlaceMap> placeMaps = new List<PlaceMap>() { placeMap1, placeMap2 };
            placeMaps.Sort();
            if (placeMaps[1].StudyTime.Start - placeMaps[0].StudyTime.End <= timeDelta)
            {
                DateTime Start = placeMaps[0].StudyTime.End;
                DateTime End = placeMaps[1].StudyTime.Start;
                Place PlaceStart = placeMaps[0].Place;
                Place PlaceEnd = placeMaps[1].Place;
                return new PlaceAdjacent(Start, End, PlaceStart, PlaceEnd);
            }
            return null;
        }
    }

    /// <summary>
    /// Đại diện cho xung đột vị trí của hai giờ học đầu và hai giờ sau.
    /// </summary>
    public class PlaceAdjacent
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public Place PlaceStart { get; set; }
        public Place PlaceEnd { get; set; }

        public PlaceAdjacent(DateTime start, DateTime end, Place placeStart, Place placeEnd)
        {
            Start = start;
            End = end;
            PlaceStart = placeStart;
            PlaceEnd = placeEnd;
        }
    }
}
