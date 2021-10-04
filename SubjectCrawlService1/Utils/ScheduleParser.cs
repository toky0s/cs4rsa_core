using HtmlAgilityPack;
using SubjectCrawlService1.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SubjectCrawlService1.Utils
{
    /// <summary>
    /// Bộ phân tích này dùng để chuyển một <td> tag thành Schedule [{'T2:': ['07:00-09:00', '07:00-10:15']}]
    /// </summary>
    public class ScheduleParser
    {
        private readonly HtmlNode tdTag;
        public ScheduleParser(HtmlNode tdTag)
        {
            this.tdTag = tdTag;
            CleanTdTag();
        }

        /// <summary>
        /// Xoá phần lịch học bổ xung.
        /// </summary>
        /// <param name="tdTag">tdTag chứa thông tin thời gian học.</param>
        /// <returns></returns>
        private void CleanTdTag()
        {
            HtmlNode needRemoveNode = tdTag.SelectSingleNode("//div[contains(@style, 'color: red; padding-top: 2px; text-align: center; position: relative')]");
            if (needRemoveNode == null)
            {
                return;
            }
            needRemoveNode.Remove();
        }

        public Schedule ToSchedule()
        {
            string[] dataFromTrTag = ExtractDataFromTrTag(tdTag);
            string[] times = CleanTimeItem(dataFromTrTag);
            // Convert to generic data;
            Dictionary<DayOfWeek, List<StudyTime>> scheduleTime = new Dictionary<DayOfWeek, List<StudyTime>>();
            Regex regexDate = new Regex(@"^T[2-7]:$|^CN:$");
            Regex regexTime = new Regex(@"^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$");

            int i = 0;
            DayOfWeek dayOfWeek = DayOfWeek.Sunday;
            Dictionary<DayOfWeek, List<string>> dateTimeStringPairs = new Dictionary<DayOfWeek, List<string>>();
            while (i < times.Length)
            {
                if (regexDate.IsMatch(times[i]))
                {
                    dayOfWeek = DateToDateWeek(times[i]);
                    List<string> timeStrings = new List<string>();
                    dateTimeStringPairs.Add(dayOfWeek, timeStrings);
                }
                if (regexTime.IsMatch(times[i]))
                {
                    dateTimeStringPairs[dayOfWeek].Add(times[i]);
                }
                i++;
            }
            // Dict[Date:List[time]]
            foreach (var item in dateTimeStringPairs)
            {
                List<string> timeStrings = item.Value;
                List<StudyTime> studyTimes = TimeStringsToListStudyTime(timeStrings);
                scheduleTime[item.Key] = studyTimes;
            }
            return new Schedule(scheduleTime);
        }

        /// <summary>
        /// Trả về array text được tách ra từ thẻ Td của thời gian học.
        /// </summary>
        /// <param name="trTag"></param>
        private string[] ExtractDataFromTrTag(HtmlNode tdTag)
        {
            string[] separatingStrings = { " ", "\n", "\r", "-", "," };
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

        /// <summary>
        /// Chuyển một chuỗi thời gian thành một List StudyTime.
        /// </summary>
        /// <param name="timeStrings">Time string phải match với pattern ^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$</param>
        /// <returns></returns>
        private List<StudyTime> TimeStringsToListStudyTime(List<string> timeStrings)
        {
            timeStrings = timeStrings.Distinct().ToList();
            List<StudyTime> studyTimes = new List<StudyTime>();
            int i = 0;
            while (i < timeStrings.Count())
            {
                StudyTime studyTime = new StudyTime(timeStrings[i], timeStrings[i + 1]);
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
        private List<int> IndexDayOfWeek(string[] times)
        {
            Regex regex = new Regex(@"^T[2-7]:$|^CN:$");
            List<int> output = new List<int>();
            for (int i = 0; i < times.Count(); i++)
            {
                if (regex.IsMatch(times[i]))
                {
                    output.Add(i);
                }
            }
            return output;
        }

        /// <summary>
        /// Nhận vào một chuỗi đại diện cho một Thứ trong Tuần. Trả về một enum cho hôm đó.
        /// </summary>
        /// <param name="date">T2: T4: T5: T6: T7: CN:</param>
        /// <returns>Enum Week.</returns>
        private DayOfWeek DateToDateWeek(string date)
        {
            switch (date)
            {
                case "T2:":
                    return DayOfWeek.Monday;
                case "T3:":
                    return DayOfWeek.Tuesday;
                case "T4:":
                    return DayOfWeek.Wednesday;
                case "T5:":
                    return DayOfWeek.Thursday;
                case "T6:":
                    return DayOfWeek.Friday;
                case "T7:":
                    return DayOfWeek.Saturday;
            }
            return DayOfWeek.Sunday;
        }
    }
}
