using SubjectCrawlService1.DataTypes.Enums;
using System;

namespace ConflictService.DataTypes
{
    /// <summary>
    /// Đại diện cho xung đột vị trí của hai giờ học đầu và hai giờ sau.
    /// 
    /// Ngang cấp với ConflictTime.
    /// </summary>
    public class PlaceAdjacent
    {
        public readonly DateTime Start;
        public readonly DateTime End;

        public readonly Place PlaceStart;
        public readonly Place PlaceEnd;

        public string StartAsString { get { return Start.ToString("HH:mm"); } }

        public string EndAsString { get { return End.ToString("HH:mm"); } }

        public PlaceAdjacent(DateTime start, DateTime end, Place placeStart, Place placeEnd)
        {
            Start = start;
            End = end;
            PlaceStart = placeStart;
            PlaceEnd = placeEnd;
        }
    }
}
