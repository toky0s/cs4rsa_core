

using ConflictService.DataTypes;
using SubjectCrawlService1.DataTypes;
using System;

namespace cs4rsa_core.Models
{
    /// <summary>
    /// Đại diện cho một ô trong ScheduleControl.
    /// </summary>
    public class TimeBlock
    {
        public string Background { get; set; }
        public string Desciption { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
