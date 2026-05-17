using Cs4rsa.Service.Conflict.DataTypes.Enums;
using Cs4rsa.Service.Conflict.Models;
using Cs4rsa.Service.Conflict.Utils;
using Cs4rsa.Service.SubjectCrawler.DataTypes;
using Cs4rsa.Service.SubjectCrawler.DataTypes.Enums;
using Cs4rsa.Service.SubjectCrawler.Utils;
using Cs4rsa.Services.ConflictSvc.Utils;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Cs4rsa.Service.Conflict.DataTypes
{
    public class Conflict : BaseConflict
    {
        public ConflictType ConflictType => ConflictType.Time;
        private ConflictTime _conflictTime;

        public ConflictTime ConflictTime => _conflictTime ?? (_conflictTime = GetConflictTime());

        public Conflict(Lesson lessonA, Lesson lessonB) : base(lessonA, lessonB)
        {
        }

        private ConflictTime GetConflictTime()
        {
            // Check phase
            PhaseIntersect phaseIntersect = PhaseManipulation.GetPhaseIntersect(LessonA.StudyWeek, LessonB.StudyWeek);
            if (phaseIntersect.Equals(PhaseIntersect.NullInstance))
            {
                return null;
            }

            Schedule scheduleClassGroup1 = LessonA.Schedule;
            Schedule scheduleClassGroup2 = LessonB.Schedule;
            IEnumerable<DayOfWeek> dayOfWeeks = ScheduleManipulation.GetIntersectDate(scheduleClassGroup1, scheduleClassGroup2);

            // Check date
            if (!dayOfWeeks.Any())
            {
                return null;
            }

            Dictionary<DayOfWeek, IEnumerable<StudyTimeIntersect>> conflictTimes = new Dictionary<DayOfWeek, IEnumerable<StudyTimeIntersect>>();

            // Check time
            foreach (DayOfWeek dayOfWeek in dayOfWeeks)
            {
                IEnumerable<StudyTime> studyTimesClassGroup1 = scheduleClassGroup1.GetStudyTimesAtDay(dayOfWeek);
                IEnumerable<StudyTime> studyTimesClassGroup2 = scheduleClassGroup2.GetStudyTimesAtDay(dayOfWeek);
                IEnumerable<StudyTime> studyTimeJoin = studyTimesClassGroup1.Concat(studyTimesClassGroup2);
                IEnumerable<Tuple<StudyTime, StudyTime>> studyTimePairs = StudyTimeManipulation.PairStudyTimes(studyTimeJoin.ToList());
                IEnumerable<StudyTimeIntersect> studyTimeIntersects = StudyTimeManipulation.GetStudyTimeIntersects(studyTimePairs);
                if (studyTimeIntersects.Any())
                {
                    conflictTimes.Add(dayOfWeek, studyTimeIntersects);
                }
            }
            return conflictTimes.Count != 0
                ? new ConflictTime(conflictTimes)
                : null;
        }

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
    }
}
