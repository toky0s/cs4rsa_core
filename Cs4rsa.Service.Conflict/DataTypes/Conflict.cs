using Cs4rsa.Service.Conflict.Models;
using Cs4rsa.Service.SubjectCrawler.DataTypes;
using Cs4rsa.Services.ConflictSvc.Utils;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Cs4rsa.Service.Conflict.DataTypes
{
    public class Conflict : BaseConflict
    {
        public Conflict(Lesson lessonA, Lesson lessonB) : base(lessonA, lessonB)
        {
        }

        public ConflictTime GetConflictTime()
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
    }
}
