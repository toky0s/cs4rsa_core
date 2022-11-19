using System;
using System.Collections.Generic;

namespace Cs4rsa.Services.ConflictSvc.DataTypes
{
    /// <summary>
    /// ConflictTime đại diện cho một khoảng thời gian xung đột giữa hai ClassGroup.
    /// ConflictTime sẽ bao gồm các một Dict với các key là các DayOfWeek có xung đột
    /// và các value là StudyTimeIntersect đại diện cho khoảng thời gian gây xung đột trong thứ đó.
    /// 
    /// Ngang cấp với PlaceAdjacent.
    /// </summary>
    public readonly struct ConflictTime
    {
        public static readonly ConflictTime NullInstance = new();

        public readonly Dictionary<DayOfWeek, IEnumerable<StudyTimeIntersect>> ConflictTimes;

        public ConflictTime(Dictionary<DayOfWeek, IEnumerable<StudyTimeIntersect>> conflictTimes)
        {
            ConflictTimes = conflictTimes;
        }
    }
}
