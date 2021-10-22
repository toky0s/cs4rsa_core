using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConflictService.DataTypes
{
    /// <summary>
    /// ConflictTime đại diện cho một khoảng thời gian xung đột giữa hai ClassGroup.
    /// ConflictTime sẽ bao gồm các một Dict với các key là các DayOfWeek có xung đột
    /// và các value là StudyTimeIntersect đại diện cho khoảng thời gian gây xung đột trong thứ đó.
    /// </summary>
    public class ConflictTime
    {
        private readonly Dictionary<DayOfWeek, List<StudyTimeIntersect>> conflictTimes = new Dictionary<DayOfWeek, List<StudyTimeIntersect>>();
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
            List<DayOfWeek> dayOfWeeks = new List<DayOfWeek>();
            foreach (DayOfWeek dayOfWeek in conflictTimes.Keys)
            {
                dayOfWeeks.Add(dayOfWeek);
            }
            return dayOfWeeks;
        }
    }

}
