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
                List<DayOfWeek> DayOfWeeks = ScheduleManipulation.GetIntersectDate(scheduleClassGroup1, scheduleClassGroup2);
                // Check date
                if (DayOfWeeks.Count > 0)
                {
                    Dictionary<DayOfWeek, List<StudyTimeIntersect>> conflictTimes = new Dictionary<DayOfWeek, List<StudyTimeIntersect>>();
                    //check time
                    foreach (DayOfWeek DayOfWeek in DayOfWeeks)
                    {
                        List<StudyTime> studyTimesClassGroup1 = scheduleClassGroup1.GetStudyTimesAtDay(DayOfWeek);
                        List<StudyTime> studyTimesClassGroup2 = scheduleClassGroup2.GetStudyTimesAtDay(DayOfWeek);
                        List<StudyTime> studyTimeJoin = studyTimesClassGroup1.Concat(studyTimesClassGroup2).ToList();
                        List<Tuple<StudyTime, StudyTime>> studyTimePairs = StudyTimeManipulation.PairStudyTimes(studyTimeJoin);
                        List<StudyTimeIntersect> studyTimeIntersects = StudyTimeManipulation.GetStudyTimeIntersects(studyTimePairs);
                        if (studyTimeIntersects.Count != 0)
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
