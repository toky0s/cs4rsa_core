using cs4rsa_core.Services.SubjectCrawlerSvc.DataTypes.Enums;

using System;
using System.Globalization;

namespace cs4rsa_core.Services.ConflictSvc.DataTypes
{
    /// <summary>
    /// Đại diện cho xung đột vị trí của hai giờ học đầu và hai giờ sau.
    /// 
    /// Ngang cấp với ConflictTime.
    /// </summary>
    public struct PlaceAdjacent
    {
        public static readonly PlaceAdjacent Instance = new();

        public readonly DateTime Start;
        public readonly DateTime End;

        public readonly Place PlaceStart;
        public readonly Place PlaceEnd;

        public readonly string StartAsString;
        public readonly string EndAsString;

        public PlaceAdjacent(DateTime start, DateTime end, Place placeStart, Place placeEnd)
        {
            Start = start;
            End = end;
            PlaceStart = placeStart;
            PlaceEnd = placeEnd;
            StartAsString = Start.ToString("HH:mm", CultureInfo.CurrentCulture);
            EndAsString = End.ToString("HH:mm", CultureInfo.CurrentCulture);
        }

        public static PlaceAdjacent Build()
        {
            return Instance;
        }
    }
}
