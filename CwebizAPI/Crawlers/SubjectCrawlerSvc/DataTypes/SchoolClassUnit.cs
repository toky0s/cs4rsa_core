using CwebizAPI.Crawlers.TeacherCrawlerSvc.Models;

namespace CwebizAPI.Crawlers.SubjectCrawlerSvc.DataTypes
{
    /// <summary>
    /// Tiết học.
    /// </summary>
    /// <remarks>
    /// Đại diện cho một tiết học duy nhất
    /// với thời gian học duy nhất
    /// nơi học duy nhất
    /// phòng học duy nhất.
    /// Đồng thời giữ một tham chiếu tới SchoolClass gốc.
    /// </remarks>
    public class SchoolClassUnit
    {
        public SchoolClassUnit(
            SchoolClass schoolClass,
            DayOfWeek dayOfWeek, 
            DateTime start, 
            DateTime end, 
            Room room, 
            StudyWeek studyWeek,
            string className)
        {
            SchoolClass = schoolClass;
            DayOfWeek = dayOfWeek;
            Start = start;
            End = end;
            Room = room;
            StudyWeek = studyWeek;
            ClassName = className;
        }

        public SchoolClass SchoolClass { get; }
        public DayOfWeek DayOfWeek { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public Room Room { get; set; }
        public StudyWeek StudyWeek { get; set; }
        public IEnumerable<TeacherModel> Teachers { get; set; }
        public string ClassName { get; set; }
    }
}
