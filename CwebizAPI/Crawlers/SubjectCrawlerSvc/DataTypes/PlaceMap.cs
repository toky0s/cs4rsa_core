using CwebizAPI.Crawlers.SubjectCrawlerSvc.DataTypes.Enums;

namespace CwebizAPI.Crawlers.SubjectCrawlerSvc.DataTypes
{
    /// <summary>
    /// PlaceMap đại diện cho một khối thời gian đi kèm vị trí nơi học.
    /// </summary>
    public class PlaceMap : IComparable<PlaceMap>
    {
        public StudyTime StudyTime { get; set; }
        public Place Place { get; set; }
        public SchoolClass SchoolClass { get; set; }

        public PlaceMap(
            StudyTime studyTime,
            Place place,
            SchoolClass schoolClass)
        {
            StudyTime = studyTime;
            Place = place;
            SchoolClass = schoolClass;
        }

        public int CompareTo(PlaceMap other)
        {
            return StudyTime.Start >= other.StudyTime.Start ? 1 : -1;
        }
    }
}
