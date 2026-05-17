using Cs4rsa.Service.Conflict.DataTypes;
using Cs4rsa.Service.Conflict.DataTypes.Enums;
using Cs4rsa.Service.Conflict.Interfaces;
using Cs4rsa.Service.Conflict.Models;
using Cs4rsa.Service.SubjectCrawler.DataTypes.Enums;
using Cs4rsa.Service.SubjectCrawler.Utils;
using Cs4rsa.UI.ScheduleTable.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Cs4rsa.UI.ScheduleTable.Models
{
    public class ConflictModel : IConflictModel
    {
        public ConflictTime ConflictTime { get; }
        public Lesson LessonA { get; }
        public Lesson LessonB { get; }
        public string ConflictInfo { get; }

        public ConflictType ConflictType => ConflictType.Time;

        public Phase Phase { get; }

        public ConflictModel(Conflict conflict)
        {
            LessonA = conflict.LessonA;
            LessonB = conflict.LessonB;
            ConflictTime = conflict.GetConflictTime();
            ConflictInfo = ToString();
            Phase = GetPhase();
        }

        /// <summary>
        /// Lấy ra thông tin dạng chuỗi để hiển thị lên giao diện của một xung đột về thời gian.
        /// </summary>
        public override string ToString()
        {
            List<string> resultTimes = new List<string>();
            foreach (KeyValuePair<DayOfWeek, IEnumerable<StudyTimeIntersect>> item in ConflictTime.ConflictTimes)
            {
                string day = item.Key.ToCs4rsaVietnamese();
                List<string> times = new List<string>();
                foreach (StudyTimeIntersect studyTimeIntersect in item.Value)
                {
                    string time = $"Từ {studyTimeIntersect.StartString} đến {studyTimeIntersect.EndString}";
                    times.Add(time);
                }
                string timeString = string.Join("\n", times);
                resultTimes.Add(day + "\n" + timeString);
            }
            return string.Join("\n", resultTimes);
        }

        private Phase GetPhase()
        {
            if (LessonA.Phase == Phase.First && LessonB.Phase == Phase.First ||
                    LessonA.Phase == Phase.First && LessonB.Phase == Phase.All ||
                    LessonA.Phase == Phase.All && LessonB.Phase == Phase.First)
                return Phase.First;
            if (LessonA.Phase == Phase.Second && LessonB.Phase == Phase.Second ||
                    LessonA.Phase == Phase.Second && LessonB.Phase == Phase.All ||
                    LessonA.Phase == Phase.All && LessonB.Phase == Phase.Second)
                return Phase.Second;
            return Phase.All;
        }
    }
}
