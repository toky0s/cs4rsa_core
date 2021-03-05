using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.BasicData
{
    public class StudyTime
    {
        private DateTime start;
        private DateTime end;
        public DateTime Start {get { return start; } set { start = value; }}
        public DateTime End { get { return end; } set { end = value; } }

        public StudyTime()
        {

        }

        public StudyTime(DateTime start, DateTime end)
        {
            this.start = start;
            this.end = end;
        }

        public StudyTime(string start, string end)
        {
            this.start = DateTime.ParseExact(start, "HH:mm", System.Globalization.CultureInfo.InvariantCulture);
            this.end = DateTime.ParseExact(end, "HH:mm", System.Globalization.CultureInfo.InvariantCulture);
        }
    }


    /// <summary>
    /// Đại điện cho một khoảng giao về thời gian giữa hai StudyTime. Phục vụ cho việc phát hiện xung đột.
    /// </summary>
    public class StudyTimeIntersect
    {
        private readonly DateTime start;
        private readonly DateTime end;

        public string Start { get { return start.ToString("HH:mm"); } }
        public string End { get { return end.ToString("HH:mm"); } }

        public StudyTimeIntersect(DateTime start, DateTime end)
        {
            this.start = start;
            this.end = end;
        }
    }

    /// <summary>
    /// Bao gồm các phương thức thao tác với StudyTime.
    /// </summary>
    public class StudyTimeManipulation
    {
        /// <summary>
        /// Trả về khoảng thời gian giao nhau giữa hai StudyTime, nếu chúng không giao nhau trả về null.
        /// </summary>
        /// <param name="studyTime1">StudyTime.</param>
        /// <param name="studyTime2">StudyTime.</param>
        /// <returns>StudyTimeIntersect đại diện cho một khoảng giao về thời gian giữa hai StudyTime. Phục vụ cho việc phát hiện xung đột.</returns>
        public static StudyTimeIntersect GetStudyTimeIntersect(StudyTime studyTime1, StudyTime studyTime2)
        {
            List<DateTime> studyTimes = new List<DateTime>
            {
                studyTime1.Start,
                studyTime1.End,
                studyTime2.Start,
                studyTime2.End
            };
            studyTimes.Sort();
            if (studyTimes[1] == studyTime2.Start 
                && studyTimes[2] == studyTime1.End
                && studyTime2.Start < studyTime1.End)
            {
                return new StudyTimeIntersect(studyTime2.Start, studyTime1.End);
            }
            if (studyTimes[1] == studyTime1.Start
                && studyTimes[2] == studyTime2.End
                && studyTime1.Start < studyTime2.End)
            {
                return new StudyTimeIntersect(studyTime1.Start, studyTime2.End);
            }
            if (studyTimes[0] == studyTime1.Start && studyTimes[3] == studyTime1.End)
            {
                return new StudyTimeIntersect(studyTime2.Start, studyTime2.End);
            }
            if (studyTimes[0] == studyTime2.Start && studyTimes[3] == studyTime2.End)
            {
                return new StudyTimeIntersect(studyTime1.Start, studyTime1.End);
            }
            if (studyTimes[0] == studyTime2.Start
                && studyTimes[2] == studyTime2.End
                && studyTime2.End < studyTime1.End)
            {
                return new StudyTimeIntersect(studyTime2.Start, studyTime2.End);
            }
            if (studyTimes[0] == studyTime1.Start
                && studyTimes[2] == studyTime1.End
                && studyTime1.End < studyTime2.End)
            {
                return new StudyTimeIntersect(studyTime1.Start, studyTime1.End);
            }
            return null;
        }

        /// <summary>
        /// Bắt cặp các StudyTime trong cùng một List.
        /// </summary>
        /// <param name="studyTimes">List các StudyTime.</param>
        /// <returns>List các Tuple là cặp các StudyTime.</returns>
        public static List<Tuple<StudyTime, StudyTime>> PairStudyTimes(List<StudyTime> studyTimes)
        {
            List<Tuple<StudyTime, StudyTime>> tupleStudyTimes = new List<Tuple<StudyTime, StudyTime>>();
            int index = 0;
            while (index < studyTimes.Count() - 1)
            {
                StudyTime firstItem = studyTimes[index];
                for(int j = index+1; j <= studyTimes.Count()-1; ++j)
                {
                    Tuple<StudyTime, StudyTime> tupleStudyTime = new Tuple<StudyTime, StudyTime>(firstItem, studyTimes[j]);
                    tupleStudyTimes.Add(tupleStudyTime);
                }
                index++;
            }
            return tupleStudyTimes;
        }

        /// <summary>
        /// Lấy ra các StudyTimeIntersect từ List các Tuple StudyTime.
        /// </summary>
        /// <param name="studyTimeTuples">List các Tuple StudyTime.</param>
        /// <returns>Danh sách các StudyTimeIntersect.</returns>
        public static List<StudyTimeIntersect> GetStudyTimeIntersects(List<Tuple<StudyTime, StudyTime>> studyTimeTuples)
        {
            List<StudyTimeIntersect> studyTimeIntersects = new List<StudyTimeIntersect>();
            foreach(Tuple<StudyTime, StudyTime> item in studyTimeTuples)
            {
                StudyTimeIntersect studyTimeIntersect = GetStudyTimeIntersect(item.Item1, item.Item2);
                studyTimeIntersects.Add(studyTimeIntersect);
            }
            studyTimeIntersects.RemoveAll(item => item == null);
            return studyTimeIntersects;
        }
    }
}
