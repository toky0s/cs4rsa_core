using Cs4rsaDatabaseService.Models;
using System;
using System.Collections.Generic;

namespace SubjectCrawlService1.DataTypes
{
    /**
     * Struct này đại diện cho một tiết học duy nhất
     * với thời gian học duy nhất
     * nơi học duy nhất
     * phòng học duy nhất
     */
    public class SchoolClassUnit
    {
        public DayOfWeek DayOfWeek { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public Room Room { get; set; }
        public StudyWeek StudyWeek { get; set; }
        public IEnumerable<Teacher> Teachers { get; set; }
        public string ClassName { get; set; }
    }
}
