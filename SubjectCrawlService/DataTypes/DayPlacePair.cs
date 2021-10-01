using SubjectCrawlService.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubjectCrawlService.DataTypes
{
    /// <summary>
    /// Một ngày học sẽ có một thời gian học, một nơi học và một phòng học nhất định.
    /// DayPlacePair đại diện cho điều này.
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
