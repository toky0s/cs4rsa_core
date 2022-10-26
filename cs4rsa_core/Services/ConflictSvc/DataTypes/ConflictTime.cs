using System;
using System.Collections.Generic;

namespace cs4rsa_core.Services.ConflictSvc.DataTypes
{
    /// <summary>
    /// ConflictTime đại diện cho một khoảng thời gian xung đột giữa hai ClassGroup.
    /// ConflictTime sẽ bao gồm các một Dict với các key là các DayOfWeek có xung đột
    /// và các value là StudyTimeIntersect đại diện cho khoảng thời gian gây xung đột trong thứ đó.
    /// 
    /// Ngang cấp với PlaceAdjacent.
    /// </summary>
    public class ConflictTime
    {
        private readonly Dictionary<DayOfWeek, IEnumerable<StudyTimeIntersect>> conflictTimes;
        public Dictionary<DayOfWeek, IEnumerable<StudyTimeIntersect>> ConflictTimes { get { return conflictTimes; } }

        public ConflictTime(Dictionary<DayOfWeek, IEnumerable<StudyTimeIntersect>> conflictTimes)
        {
            this.conflictTimes = conflictTimes;
        }
    }
}
