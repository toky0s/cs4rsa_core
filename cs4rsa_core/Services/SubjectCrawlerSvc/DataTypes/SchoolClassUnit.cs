using cs4rsa_core.Services.TeacherCrawlerSvc.Models;

using System;
using System.Collections.Generic;

namespace cs4rsa_core.Services.SubjectCrawlerSvc.DataTypes
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
        public IEnumerable<TeacherModel> Teachers { get; set; }
        public string ClassName { get; set; }
    }
}
