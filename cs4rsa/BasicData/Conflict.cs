using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.BasicData
{

    /// <summary>
    /// ConflictTime đại diện cho một khoảng thời gian xung đột giữa hai ClassGroup.
    /// ConflictTime sẽ bao gồm các một Dict với các key là các WeekDate có xung đột
    /// và các value là StudyTimeIntersect đại diện cho khoảng thời gian gây xung đột trong thứ đó.
    /// </summary>
    class ConflictTime
    {
        private readonly Dictionary<WeekDate, List<StudyTimeIntersect>> conflictTimes = new Dictionary<WeekDate, List<StudyTimeIntersect>>();
        public Dictionary<WeekDate, List<StudyTimeIntersect>> ConflictTimes { get { return conflictTimes; } }

        public ConflictTime(Dictionary<WeekDate, List<StudyTimeIntersect>> conflictTimes)
        {
            this.conflictTimes = conflictTimes;
        }

        public List<StudyTimeIntersect> GetStudyTimeIntersects(WeekDate weekDate)
        {
            return conflictTimes[weekDate];
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
                List<WeekDate> weekDates = ScheduleManipulation.GetIntersectDate(scheduleClassGroup1, scheduleClassGroup2);
                // Check date
                if (weekDates.Count > 0)
                {
                    Dictionary<WeekDate, List<StudyTimeIntersect>> conflictTimes = new Dictionary<WeekDate, List<StudyTimeIntersect>>();
                    //check time
                    foreach(WeekDate weekDate in weekDates)
                    {
                        List<StudyTime> studyTimesClassGroup1 = scheduleClassGroup1.GetStudyTimesAtDay(weekDate);
                        List<StudyTime> studyTimesClassGroup2 = scheduleClassGroup2.GetStudyTimesAtDay(weekDate);
                        studyTimesClassGroup1.Concat(studyTimesClassGroup2);
                        List<Tuple<StudyTime, StudyTime>> studyTimePairs = StudyTimeManipulation.PairStudyTimes(studyTimesClassGroup1);
                        List<StudyTimeIntersect> studyTimeIntersects = StudyTimeManipulation.GetStudyTimeIntersects(studyTimePairs);
                        conflictTimes.Add(weekDate, studyTimeIntersects);
                    }
                    return new ConflictTime(conflictTimes);
                }
                return null;
            }
            return null;
        }
    }
}
