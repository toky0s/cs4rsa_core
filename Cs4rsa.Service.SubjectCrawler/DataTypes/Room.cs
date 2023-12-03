using Cs4rsa.Service.SubjectCrawler.DataTypes.Enums;

namespace Cs4rsa.Service.SubjectCrawler.DataTypes
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
