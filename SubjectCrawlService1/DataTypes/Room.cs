using SubjectCrawlService1.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubjectCrawlService1.DataTypes
{
    /// <summary>
    /// Đại diện cho một phòng học.
    /// </summary>
    public class Room
    {
        public string Name { get; private set; }
        public Place Place { get; private set; }

        public Room(string name)
        {
            Name = name;
        }

        public Room(string name, Place place)
        {
            Name = name;
            Place = place;
        }
    }
}
