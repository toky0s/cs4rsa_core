using SubjectCrawlService1.DataTypes.Enums;
using System;

namespace SubjectCrawlService1.DataTypes
{
    /// <summary>
    /// Đại diện cho thứ-phòng-nơi
    /// 
    /// Thông tin này được lấy từ details page
    /// của các SchoolClass.
    /// </summary>
    public class DayRoomPlace
    {
        public DayOfWeek Day { get; private set; }
        public Room Room { get; private set; }
        public Place Place { get; private set; }

        public DayRoomPlace(DayOfWeek day, Room room, Place place)
        {
            Day = day;
            Room = room;
            Place = place;
        }
    }
}
