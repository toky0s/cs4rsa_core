using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.BaseClasses;
using cs4rsa.Models;

namespace cs4rsa.BasicData
{

    /// <summary>
    /// ConflictTime đại diện cho một khoảng thời gian xung đột giữa hai ClassGroup.
    /// ConflictTime sẽ bao gồm các một Dict với các key là các DayOfWeek có xung đột
    /// và các value là StudyTimeIntersect đại diện cho khoảng thời gian gây xung đột trong thứ đó.
    /// </summary>
    public class ConflictTime
    {
        private readonly Dictionary<DayOfWeek, List<StudyTimeIntersect>> conflictTimes = new Dictionary<DayOfWeek, List<StudyTimeIntersect>>();
        public Dictionary<DayOfWeek, List<StudyTimeIntersect>> ConflictTimes { get { return conflictTimes; } }

        public ConflictTime(Dictionary<DayOfWeek, List<StudyTimeIntersect>> conflictTimes)
        {
            this.conflictTimes = conflictTimes;
        }

        public List<StudyTimeIntersect> GetStudyTimeIntersects(DayOfWeek DayOfWeek)
        {
            return conflictTimes[DayOfWeek];
        }

        public List<DayOfWeek> GetSchoolDays()
        {
            List<DayOfWeek> dayOfWeeks = new List<DayOfWeek>();
            foreach(DayOfWeek dayOfWeek in conflictTimes.Keys)
            {
                dayOfWeeks.Add(dayOfWeek);
            }
            return dayOfWeeks;
        }
    }

    public class Conflict: BaseConflict
    {
        public Conflict(SchoolClass schoolClass1, SchoolClass schoolClass2) : base(schoolClass1, schoolClass2)
        {
        }

        public Conflict(SchoolClassModel schoolClass1, SchoolClassModel schoolClass2) : base(schoolClass1, schoolClass2)
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
                    foreach(DayOfWeek DayOfWeek in DayOfWeeks)
                    {
                        List<StudyTime> studyTimesClassGroup1 = scheduleClassGroup1.GetStudyTimesAtDay(DayOfWeek);
                        List<StudyTime> studyTimesClassGroup2 = scheduleClassGroup2.GetStudyTimesAtDay(DayOfWeek);
                        List<StudyTime> studyTimeJoin = studyTimesClassGroup1.Concat(studyTimesClassGroup2).ToList();
                        List<Tuple<StudyTime, StudyTime>> studyTimePairs = StudyTimeManipulation.PairStudyTimes(studyTimeJoin);
                        List<StudyTimeIntersect> studyTimeIntersects = StudyTimeManipulation.GetStudyTimeIntersects(studyTimePairs);
                        if (studyTimeIntersects.Count != 0)
                            conflictTimes.Add(DayOfWeek, studyTimeIntersects);
                    }
                    if (conflictTimes.Count != 0)
                        return new ConflictTime(conflictTimes);
                    return null;
                }
                return null;
            }
            return null;
        }
    }
}
