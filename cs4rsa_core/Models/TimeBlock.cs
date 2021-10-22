

using ConflictService.DataTypes;
using SubjectCrawlService1.DataTypes;

namespace cs4rsa_core.Models
{
    /// <summary>
    /// Đại diện cho một ô trong ScheduleRow. Là một ô trong bảng mô phỏng hiển thị.
    /// </summary>
    public class TimeBlock
    {
        private readonly string _defaultForeground = "#000000";
        public string Background { get; set; }
        public string Foreground { get; set; }
        public string Text { get; set; }
        public TimeBlock(SchoolClass schoolClass)
        {
            Background = schoolClass.Color;
            Foreground = _defaultForeground;
            Text = schoolClass.SchoolClassName;
        }

        public TimeBlock(StudyTimeIntersect studyTimeIntersect)
        {
            Background = "#800000";
            Foreground = "#FFFFFF";
            Text = "Xung đột";
        }
    }
}
