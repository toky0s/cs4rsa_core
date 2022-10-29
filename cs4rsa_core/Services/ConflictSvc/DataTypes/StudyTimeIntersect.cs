﻿using System;
using System.Globalization;

namespace cs4rsa_core.Services.ConflictSvc.DataTypes
{
    /// <summary>
    /// Đại điện cho một khoảng giao về thời gian giữa hai StudyTime. Phục vụ cho việc phát hiện xung đột.
    /// </summary>
    public struct StudyTimeIntersect
    {
        public static readonly StudyTimeIntersect Instance = new();

        public readonly DateTime Start;
        public readonly DateTime End;

        public readonly string StartString;
        public readonly string EndString;

        public StudyTimeIntersect(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
            StartString = start.ToString("HH:mm", CultureInfo.CurrentCulture);
            EndString = end.ToString("HH:mm", CultureInfo.CurrentCulture);
        }
    }
}