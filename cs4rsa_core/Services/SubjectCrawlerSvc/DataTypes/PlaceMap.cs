using cs4rsa_core.Services.SubjectCrawlerSvc.DataTypes.Enums;

namespace cs4rsa_core.Services.SubjectCrawlerSvc.DataTypes
{
    /// <summary>
    /// PlaceMap đại diện cho một khối thời gian đi kèm vị trí nơi học.
    /// </summary>
    public class PlaceMap
    {
        public StudyTime StudyTime { get; set; }
        public Place Place { get; set; }

        public PlaceMap(StudyTime studyTime, Place place)
        {
            StudyTime = studyTime;
            Place = place;
        }
    }
}
