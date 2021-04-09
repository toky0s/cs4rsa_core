using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.BasicData
{

    /// <summary>
    /// ConflictTime đại diện cho một khoảng thời gian xung đột giữa hai ClassGroup.
    /// ConflictTime sẽ bao gồm các một Dict với các key là các DayOfWeek có xung đột
    /// và các value là StudyTimeIntersect đại diện cho khoảng thời gian gây xung đột trong thứ đó.
    /// </summary>
    class ConflictTime
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
    }

    class Conflict
    {
        private readonly ClassGroup classGroup1;
        private readonly ClassGroup classGroup2;

        public ClassGroup ClassGroupFirst { get { return classGroup1; } }
        public ClassGroup ClassGroupSecond { get { return classGroup2; } }

        public Conflict(ClassGroup classGroup1, ClassGroup classGroup2)
        {
            this.classGroup1 = classGroup1;
            this.classGroup2 = classGroup2;
        }

        public ConflictTime GetConflictTime()
        {
            // Check phase
            if (classGroup1.GetPhase() == classGroup2.GetPhase())
            {
                Schedule scheduleClassGroup1 = classGroup1.GetSchedule();
                Schedule scheduleClassGroup2 = classGroup2.GetSchedule();
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
                        studyTimesClassGroup1.Concat(studyTimesClassGroup2);
                        List<Tuple<StudyTime, StudyTime>> studyTimePairs = StudyTimeManipulation.PairStudyTimes(studyTimesClassGroup1);
                        List<StudyTimeIntersect> studyTimeIntersects = StudyTimeManipulation.GetStudyTimeIntersects(studyTimePairs);
                        conflictTimes.Add(DayOfWeek, studyTimeIntersects);
                    }
                    return new ConflictTime(conflictTimes);
                }
                return null;
            }
            return null;
        }
    }
}
