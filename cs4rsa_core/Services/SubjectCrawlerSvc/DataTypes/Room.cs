using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes.Enums;

namespace Cs4rsa.Services.SubjectCrawlerSvc.DataTypes
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
