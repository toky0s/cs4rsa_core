using Cs4rsa.Service.SubjectCrawler.DataTypes;
using Cs4rsa.Service.SubjectCrawler.DataTypes.Enums;

using System;
using System.Globalization;

namespace Cs4rsa.Service.Conflict.DataTypes
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

        public readonly string StartAsString;
        public readonly string EndAsString;

        public readonly SchoolClass SchoolClass1;
        public readonly SchoolClass SchoolClass2;

        public PlaceAdjacent(
            DateTime start, DateTime end,
            Place placeStart, Place placeEnd,
            SchoolClass schoolClass1, SchoolClass schoolClass2)
        {
            Start = start;
            End = end;
            PlaceStart = placeStart;
            PlaceEnd = placeEnd;
            StartAsString = Start.ToString("HH:mm", CultureInfo.CurrentCulture);
            EndAsString = End.ToString("HH:mm", CultureInfo.CurrentCulture);
            SchoolClass1 = schoolClass1;
            SchoolClass2 = schoolClass2;
        }
    }
}
