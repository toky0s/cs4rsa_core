using System;
using System.Collections.Generic;

namespace ConflictService.DataTypes
{
    /// <summary>
    /// ConflictTime đại diện cho một khoảng thời gian xung đột giữa hai ClassGroup.
    /// ConflictTime sẽ bao gồm các một Dict với các key là các DayOfWeek có xung đột
    /// và các value là StudyTimeIntersect đại diện cho khoảng thời gian gây xung đột trong thứ đó.
    /// </summary>
    public class ConflictTime
    {
        private readonly Dictionary<DayOfWeek, List<StudyTimeIntersect>> conflictTimes;
        public Dictionary<DayOfWeek, List<StudyTimeIntersect>> ConflictTimes { get { return conflictTimes; } }

        public ConflictTime(Dictionary<DayOfWeek, List<StudyTimeIntersect>> conflictTimes)
        {
            this.conflictTimes = conflictTimes;
        }

        public List<StudyTimeIntersect> GetStudyTimeIntersects(DayOfWeek DayOfWeek)
        {
            return conflictTimes[DayOfWeek];
        }

        public List<DayOfWeek> GetSchoolDays()
        {
            List<DayOfWeek> dayOfWeeks = new();
            foreach (DayOfWeek dayOfWeek in conflictTimes.Keys)
            {
                dayOfWeeks.Add(dayOfWeek);
            }
            return dayOfWeeks;
        }
    }

}
