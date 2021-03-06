using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace cs4rsa.BasicData
{
    /// <summary>
    /// Đại diện cho các thứ trong một tuần học.
    /// </summary>
    public enum WeekDate
    {
        MONDAY,
        TUSEDAY,
        WEDNESDAY,
        THURDAY,
        FRIDAY,
        SATURDAY,
        SUNDAY
    }

    /// <summary>
    /// Đại diện cho thời gian học của một SchoolClass.
    /// </summary>
    public class Schedule
    {
        private Dictionary<WeekDate, List<StudyTime>> scheduleTime;
        public Dictionary<WeekDate, List<StudyTime>> ScheduleTime { get { return scheduleTime; } }

        public Schedule()
        {

        }
        
        public Schedule(Dictionary<WeekDate, List<StudyTime>> scheduleTime)
        {
            this.scheduleTime = scheduleTime;
        }

        public Schedule(HtmlNode tdTag)
        {
            string[] dataFromTrTag = ExtractDataFromTrTag(tdTag);
            string[] times = CleanTimeItem(dataFromTrTag);
            // Convert to generic data;
            scheduleTime = new Dictionary<WeekDate, List<StudyTime>>();
            Regex regexDate = new Regex(@"^T[2-7]:$|^CN:$");
            Regex regexTime = new Regex(@"^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$");

            int i = 0;
            WeekDate weekDate = new WeekDate();
            Dictionary<WeekDate, List<string>> dateTimeStringPairs = new Dictionary<WeekDate, List<string>>();
            while (i < times.Length)
            {
                if (regexDate.IsMatch(times[i]))
                {
                    weekDate = DateToDateWeek(times[i]);
                    List<string> timeStrings = new List<string>();
                    dateTimeStringPairs.Add(weekDate, timeStrings);
                }
                if (regexTime.IsMatch(times[i]))
                {
                    dateTimeStringPairs[weekDate].Add(times[i]);
                }
                i++;
            }
            // Dict[Date:List[time]]
            foreach(var item in dateTimeStringPairs)
            {
                List<string> timeStrings = item.Value;
                List<StudyTime> studyTimes = TimeStringsToListStudyTime(timeStrings);
                scheduleTime[item.Key] = studyTimes;
            }
        }

        /// <summary>
        /// Chuyển một chuỗi thời gian thành một List StudyTime.
        /// </summary>
        /// <param name="timeStrings">Time string phải match với pattern ^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$</param>
        /// <returns></returns>
        public List<StudyTime> TimeStringsToListStudyTime(List<string> timeStrings)
        {
            timeStrings = timeStrings.Distinct().ToList();
            List<StudyTime> studyTimes = new List<StudyTime>();
            int i = 0;
            while (i<timeStrings.Count())
            {
                StudyTime studyTime = new StudyTime(timeStrings[i], timeStrings[i+1]);
                studyTimes.Add(studyTime);
                i = i + 2;
            }
            return studyTimes;
        }

        /// <summary>
        /// Trả về index của các chuỗi date như T2: T3: trong list times lấy được.
        /// </summary>
        /// <param name="times">{"T2:", "15:15", "17:15", "T5:", "15:15", "17:15"}</param>
        /// <returns>Một list các index của chuỗi date.</returns>
        private List<int> IndexWeekDate(string[] times)
        {
            Regex regex = new Regex(@"^T[2-7]:$|^CN:$");
            List<int> output = new List<int>();
            for(int i=0; i<times.Count(); i++)
            {
                if (regex.IsMatch(times[i]))
                {
                    output.Add(i);
                }
            }
            return output;
        }

        /// <summary>
        /// Nhận vào một chuỗi đại diện cho một Thứ trong Tuần.
        /// </summary>
        /// <param name="date">T2: T4: T5: T6: T7: CN:</param>
        /// <returns>Enum Week.</returns>
        private WeekDate DateToDateWeek(string date)
        {
            switch (date) {
                case "T2:":
                    return WeekDate.MONDAY;
                case "T3:":
                    return WeekDate.TUSEDAY;
                case "T4:":
                    return WeekDate.WEDNESDAY;
                case "T5:":
                    return WeekDate.THURDAY;
                case "T6:":
                    return WeekDate.FRIDAY;
                case "T7:":
                    return WeekDate.SATURDAY;
            }
            return WeekDate.SUNDAY;
        }

        /// <summary>
        /// Trả về array text được tách ra từ thẻ Td của thời gian học.
        /// </summary>
        /// <param name="trTag"></param>
        private string[] ExtractDataFromTrTag(HtmlNode tdTag)
        {
            string[] separatingStrings = { " ", "\n", "\r", "-" };
            string[] trTagSplitDatas = tdTag.InnerText.Trim().Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            return trTagSplitDatas;
        }

        /// <summary>
        /// Clean data time.
        /// </summary>
        /// <param name="tdTagSplitDatas"></param>
        /// <returns>Trả về array time item đã được clean.</returns>
        private string[] CleanTimeItem(string[] tdTagSplitDatas)
        {
            Regex timeRegex = new Regex(@"^T[2-7]:|CN:|^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$");
            string[] times = tdTagSplitDatas.Where(item => timeRegex.IsMatch(item)).ToArray();
            return times;
        }
        
        public List<WeekDate> GetSchoolDays()
        {
            return scheduleTime.Keys.ToList();
        }

        public List<StudyTime> GetStudyTimesAtDay(WeekDate weekDate)
        {
            return scheduleTime[weekDate];
        }
    }


    /// <summary>
    /// Class này bao gồm các phương thức để thao tác với Schedule.
    /// </summary>
    public class ScheduleManipulation
    {

        /// <summary>
        /// Giao hai các thứ của hai Schedule. Dùng để phát hiện xung đột giữa hai Schedule.
        /// </summary>
        /// <returns>Trả về WeekDate mà cả hai Schedule cùng có.</returns>
        public static List<WeekDate> GetIntersectDate(Schedule schedule1, Schedule schedule2)
        {
            return schedule1.GetSchoolDays().Intersect(schedule2.GetSchoolDays()).ToList();
        }
    }
}
