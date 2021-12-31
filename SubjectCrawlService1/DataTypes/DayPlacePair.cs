using SubjectCrawlService1.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubjectCrawlService1.DataTypes
{
    /// <summary>
    /// Đại diện cho thứ-phòng-nơi
    /// </summary>
    public class DayPlacePair
    {
        public DayOfWeek Day { get; private set; }
        public Room Room { get; private set; }
        public Place Place { get; private set; }

        public DayPlacePair(DayOfWeek day, Room room, Place place)
        {
            Day = day;
            Room = room;
            Place = place;
        }
    }
}
