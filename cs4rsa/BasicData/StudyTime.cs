using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.BasicData
{
    public enum Session
    {
        Morning, Afternoon, Night
    }

    public class StudyTime
    {
        private readonly DateTime _start;
        private readonly DateTime _end;
        public DateTime Start {get { return _start; }}
        public DateTime End { get { return _end; }}

        private readonly string _startAsString;
        private readonly string _endAsString;
        public string StartAsString => _startAsString;
        public string EndAsString => _endAsString;

        public StudyTime()
        {

        }

        public StudyTime(DateTime start, DateTime end)
        {
            _start = start;
            _end = end;
        }

        public StudyTime(string start, string end)
        {
            _startAsString = start;
            _endAsString = end;
            _start = DateTime.ParseExact(start, "HH:mm", System.Globalization.CultureInfo.InvariantCulture);
            _end = DateTime.ParseExact(end, "HH:mm", System.Globalization.CultureInfo.InvariantCulture);
        }

        public double TotalHours()
        {
            return (_end - _start).TotalHours;
        }

        public Session GetSession()
        {
            if (!IsInMorning())
            {
                if (!IsInAfternoon())
                {
                    return Session.Night;
                }
                return Session.Afternoon;
            }
            return Session.Morning;
        }


        /// <summary>
        /// Kiểm tra đây có phải là buổi sáng hay không.
        /// </summary>
        /// <returns></returns>
        private bool IsInMorning()
        {
            DateTime[] MorningTime =  {
                DateTime.ParseExact("07:00", "HH:mm", System.Globalization.CultureInfo.InvariantCulture),
                DateTime.ParseExact("11:15", "HH:mm", System.Globalization.CultureInfo.InvariantCulture)
            };
            if (_start <= MorningTime[1])
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Kiểm tra đây có phải là buổi chiều hay không.
        /// </summary>
        /// <returns></returns>
        private bool IsInAfternoon()
        {
            DateTime[] AfternoonTime =  {
                DateTime.ParseExact("13:00", "HH:mm", System.Globalization.CultureInfo.InvariantCulture),
                DateTime.ParseExact("17:15", "HH:mm", System.Globalization.CultureInfo.InvariantCulture)
            };
            if (_start >= AfternoonTime[0] && _start <= AfternoonTime[1])
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Kiểm tra đây có phải là buổi tối hay không.
        /// </summary>
        /// <returns></returns>
        private bool IsInNight()
        {
            DateTime[] NightTime =  {
                DateTime.ParseExact("17:45", "HH:mm", System.Globalization.CultureInfo.InvariantCulture),
                DateTime.ParseExact("21:00", "HH:mm", System.Globalization.CultureInfo.InvariantCulture)
            };
            if (_start >= NightTime[0] && _end <= NightTime[1])
            {
                return true;
            }
            return false;
        }
    }


    /// <summary>
    /// Đại điện cho một khoảng giao về thời gian giữa hai StudyTime. Phục vụ cho việc phát hiện xung đột.
    /// </summary>
    public class StudyTimeIntersect
    {
        private readonly DateTime _start;
        private readonly DateTime _end;

        public string StartString { get { return _start.ToString("HH:mm"); } }
        public string EndString { get { return _end.ToString("HH:mm"); } }

        public DateTime Start => _start;
        public DateTime End => _end;

        public StudyTimeIntersect(DateTime start, DateTime end)
        {
            _start = start;
            _end = end;
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
