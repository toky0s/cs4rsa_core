using Cs4rsa.Services.ConflictSvc.Utils;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes;
using Cs4rsa.Services.SubjectCrawlerSvc.Models;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Cs4rsa.Services.ConflictSvc.DataTypes
{
    public class Conflict : BaseConflict
    {
        public Conflict(SchoolClassModel schoolClass1, SchoolClassModel schoolClass2) : base(schoolClass1, schoolClass2)
        {
        }

        public ConflictTime GetConflictTime()
        {
            // Check phase
            PhaseIntersect phaseIntersect = PhaseManipulation.GetPhaseIntersect(_schoolClass1.StudyWeek, _schoolClass2.StudyWeek);
            if (phaseIntersect.Equals(PhaseIntersect.NullInstance))
            {
                return null;
            }

            Schedule scheduleClassGroup1 = _schoolClass1.Schedule;
            Schedule scheduleClassGroup2 = _schoolClass2.Schedule;
            IEnumerable<DayOfWeek> dayOfWeeks = ScheduleManipulation.GetIntersectDate(scheduleClassGroup1, scheduleClassGroup2);

            // Check date
            if (!dayOfWeeks.Any())
            {
                return null;
            }

            Dictionary<DayOfWeek, IEnumerable<StudyTimeIntersect>> conflictTimes = new();


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
