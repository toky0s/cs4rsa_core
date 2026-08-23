using Cs4rsa.Service.SubjectCrawler.DataTypes;

using System;
using System.Globalization;

namespace Cs4rsa.Service.Conflict.DataTypes
{
    /// <summary>
    /// Đại điện cho một khoảng giao về thời gian giữa hai StudyTime.
    /// </summary>
    public class StudyTimeIntersect : IEquatable<StudyTimeIntersect>
    {
        public DateTime Start { get; }
        public DateTime End { get; }

        public string StartString { get; }
        public string EndString { get; }

        public StudyTime FScStudyTime { get; }
        public StudyTime SScStudyTime { get; }

        public StudyTimeIntersect(
            DateTime start, DateTime end, 
            StudyTime fScStudyTime, StudyTime sScStudyTime)
        {
            FScStudyTime = fScStudyTime;
            SScStudyTime = sScStudyTime;
            Start = start;
            End = end;
            StartString = start.ToString("HH:mm", CultureInfo.CurrentCulture);
            EndString = end.ToString("HH:mm", CultureInfo.CurrentCulture);
        }

        public bool Equals(StudyTimeIntersect other)
        {
            return other != null
                && Start.Equals(other.Start)
                && End.Equals(other.End)
                && StartString.Equals(other.StartString)
                && EndString.Equals(other.EndString)
                && FScStudyTime.Equals(other.FScStudyTime)
                && SScStudyTime.Equals(other.SScStudyTime);
        }
    }
}
