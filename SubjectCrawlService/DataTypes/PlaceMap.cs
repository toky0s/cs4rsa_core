using SubjectCrawlService.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubjectCrawlService.DataTypes
{
    /// <summary>
    /// PlaceMap đại diện cho một khối thời gian đi kèm vị trí nơi học.
    /// </summary>
    public class PlaceMap : IComparable<PlaceMap>
    {
        public StudyTime StudyTime { get; set; }
        public Place Place { get; set; }

        public PlaceMap(StudyTime studyTime, Place place)
        {
            StudyTime = studyTime;
            Place = place;
        }

        public int CompareTo(PlaceMap other)
        {
            return StudyTime.Start.CompareTo(other.StudyTime.Start);
        }

        public override string ToString()
        {
            return $"{Place} {StudyTime}";
        }
    }
}
