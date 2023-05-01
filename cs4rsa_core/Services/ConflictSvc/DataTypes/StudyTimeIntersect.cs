using Cs4rsa.Constants;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes;

using System;
using System.Globalization;

namespace Cs4rsa.Services.ConflictSvc.DataTypes
{
    /// <summary>
    /// Đại điện cho một khoảng giao về thời gian giữa hai StudyTime. Phục vụ cho việc phát hiện xung đột.
    /// </summary>
    public class StudyTimeIntersect
    {
        public static readonly StudyTimeIntersect Instance = new();

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
            StartString = start.ToString(VmConstants.TimeFormat, CultureInfo.CurrentCulture);
            EndString = end.ToString(VmConstants.TimeFormat, CultureInfo.CurrentCulture);
        }

        public StudyTimeIntersect() { }
    }
}
