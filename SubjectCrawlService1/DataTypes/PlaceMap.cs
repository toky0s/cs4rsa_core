using SubjectCrawlService1.DataTypes.Enums;
using System;

namespace SubjectCrawlService1.DataTypes
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
