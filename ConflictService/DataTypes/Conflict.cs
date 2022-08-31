using ConflictService.Utils;

using SubjectCrawlService1.DataTypes;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ConflictService.DataTypes
{
    public class Conflict : BaseConflict
    {
        public Conflict(SchoolClass schoolClass1, SchoolClass schoolClass2) : base(schoolClass1, schoolClass2)
        {
        }

        public ConflictTime GetConflictTime()
        {
            // Check phase
            if (CanConflictPhase(_schoolClass1.GetPhase(), _schoolClass2.GetPhase()))
            {
                Schedule scheduleClassGroup1 = _schoolClass1.Schedule;
                Schedule scheduleClassGroup2 = _schoolClass2.Schedule;
                IEnumerable<DayOfWeek> DayOfWeeks = ScheduleManipulation.GetIntersectDate(scheduleClassGroup1, scheduleClassGroup2);
                // Check date
                if (DayOfWeeks.Any())
                {
                    Dictionary<DayOfWeek, IEnumerable<StudyTimeIntersect>> conflictTimes = new();
                    //check time
                    foreach (DayOfWeek DayOfWeek in DayOfWeeks)
                    {
                        IEnumerable<StudyTime> studyTimesClassGroup1 = scheduleClassGroup1.GetStudyTimesAtDay(DayOfWeek);
                        IEnumerable<StudyTime> studyTimesClassGroup2 = scheduleClassGroup2.GetStudyTimesAtDay(DayOfWeek);
                        IEnumerable<StudyTime> studyTimeJoin = studyTimesClassGroup1.Concat(studyTimesClassGroup2);
                        IEnumerable<Tuple<StudyTime, StudyTime>> studyTimePairs = StudyTimeManipulation.PairStudyTimes(studyTimeJoin.ToList());
                        IEnumerable<StudyTimeIntersect> studyTimeIntersects = StudyTimeManipulation.GetStudyTimeIntersects(studyTimePairs);
                        if (studyTimeIntersects.Count() != 0)
                            conflictTimes.Add(DayOfWeek, studyTimeIntersects);
                    }
                    return conflictTimes.Count != 0 ? new ConflictTime(conflictTimes) : null;
                }
                return null;
            }
            return null;
        }
    }
}
