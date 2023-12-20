using Cs4rsa.Service.SubjectCrawler.DataTypes;

using System;
using System.Globalization;

namespace Cs4rsa.Service.Conflict.DataTypes
{
    /// <summary>
    /// Đại điện cho một khoảng giao về thời gian giữa hai StudyTime. Phục vụ cho việc phát hiện xung đột.
    /// </summary>
    public class StudyTimeIntersect
    {
        public static readonly StudyTimeIntersect Instance = new StudyTimeIntersect();

        public readonly DateTime Start;
        public readonly DateTime End;

        public readonly string StartString;
        public readonly string EndString;

        public StudyTime FScStudyTime { get; }
        public StudyTime SScStudyTime { get; }

        public StudyTimeIntersect(
              DateTime start
            , DateTime end
            , StudyTime fScStudyTime
            , StudyTime sScStudyTime)
        {
            FScStudyTime = fScStudyTime;
            SScStudyTime = sScStudyTime;
            Start = start;
            End = end;
            StartString = start.ToString("HH:mm", CultureInfo.CurrentCulture);
            EndString = end.ToString("HH:mm", CultureInfo.CurrentCulture);
        }

        public StudyTimeIntersect() { }
    }
}
