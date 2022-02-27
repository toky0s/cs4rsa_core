using SubjectCrawlService1.DataTypes.Enums;

namespace SubjectCrawlService1.DataTypes
{
    /// <summary>
    /// Đại diện cho một phòng học.
    /// </summary>
    public class Room
    {
        public string Name { get; private set; }
        public Place Place { get; set; }

        public Room(string name)
        {
            Name = name;
        }
    }
}
