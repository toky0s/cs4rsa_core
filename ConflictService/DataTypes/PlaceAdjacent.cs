using SubjectCrawlService1.DataTypes.Enums;
using System;

namespace ConflictService.DataTypes
{
    /// <summary>
    /// Đại diện cho xung đột vị trí của hai giờ học đầu và hai giờ sau.
    /// </summary>
    public class PlaceAdjacent
    {
        private DateTime _start { get; set; }
        private DateTime _end { get; set; }
        public string Start { get { return _start.ToString("HH:mm"); } }
        public string End { get { return _end.ToString("HH:mm"); } }
        public Place PlaceStart { get; set; }
        public Place PlaceEnd { get; set; }

        public PlaceAdjacent(DateTime start, DateTime end, Place placeStart, Place placeEnd)
        {
            _start = start;
            _end = end;
            PlaceStart = placeStart;
            PlaceEnd = placeEnd;
        }
    }
}
