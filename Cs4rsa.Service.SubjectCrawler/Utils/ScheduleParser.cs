using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;

using Cs4rsa.Service.SubjectCrawler.DataTypes;

using HtmlAgilityPack;

namespace Cs4rsa.Service.SubjectCrawler.Utils
{
    /// <summary>
    /// Bộ phân tích này dùng để chuyển một td tag thành Schedule [{'T2:': ['07:00-09:00', '07:00-10:15']}]
    /// </summary>
    public class ScheduleParser
    {
        private readonly HtmlNode _tdTag;
        public ScheduleParser(HtmlNode tdTag)
        {
            _tdTag = tdTag;
        }

        /// <summary>
        /// Xoá phần lịch học bổ xung.
        /// </summary>
        /// <param name="tdTag">tdTag chứa thông tin thời gian học.</param>
        /// <returns></returns>
        private SupplementaryClassSchedule[] GetSupplementaryClassSchedules()
        {
            // TODO: https://courses.duytan.edu.vn/Sites/Home_ChuongTrinhDaoTao.aspx?p=home_listcoursedetail&courseid=1449&timespan=93&t=s
            // Supplementary Class Schedule
            var needRemoveNode = _tdTag.SelectSingleNode("//div[contains(@style, 'color: red; padding-top: 2px; text-align: center; position: relative')]");
            if (needRemoveNode != null)
            {
                var span = _tdTag.SelectSingleNode(@"//span[@class='content']");
                return span.InnerHtml
                    .Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(GetSupplementaryClassSchedule)
                    .ToArray();
            }
            return null;
        }

        private SupplementaryClassSchedule GetSupplementaryClassSchedule(string txt)
        {
            string pattern = @"(\d{2}/\d{2}/\d{4}):\s*(\d{2}:\d{2})-(\d{2}:\d{2}),\s*([^,]+),\s*(.+)";

            var match = Regex.Match(txt, pattern);
            string date = match.Groups[1].Value;       // Ngày/tháng/năm
            string fromStr = match.Groups[2].Value;    // Giờ bắt đầu
            string toStr = match.Groups[3].Value;      // Giờ kết thúc
            string room = match.Groups[4].Value;    // Ví dụ: "P.301"
            string location = match.Groups[5].Value;   // Vị trí

            // Chuyển đổi sang DateTime
            DateTime from = DateTime.ParseExact($"{date} {fromStr}", "dd/MM/yyyy HH:mm", null);
            DateTime to = DateTime.ParseExact($"{date} {toStr}", "dd/MM/yyyy HH:mm", null);
            return new SupplementaryClassSchedule(from, to, room, location);
        }

        public Schedule ToSchedule()
        {
            var dataFromTrTag = ExtractDataFromTrTag(_tdTag);
            var times = CleanTimeItem(dataFromTrTag);
            // Convert to generic data;
            var scheduleTime = new Dictionary<DayOfWeek, List<StudyTime>>();
            var regexDate = new Regex(@"^T[2-7]:$|^CN:$");
            var regexTime = new Regex(@"^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$");

            var i = 0;
            var dayOfWeek = DayOfWeek.Sunday;
            var dateTimeStringPairs = new Dictionary<DayOfWeek, List<string>>();
            while (i < times.Length)
            {
                if (regexDate.IsMatch(times[i]))
                {
                    dayOfWeek = DateToDateWeek(times[i]);
                    var timeStrings = new List<string>();
                    dateTimeStringPairs.Add(dayOfWeek, timeStrings);
                }
                if (regexTime.IsMatch(times[i]))
                {
                    dateTimeStringPairs[dayOfWeek].Add(times[i]);
                }
                i++;
            }
            foreach (var item in dateTimeStringPairs)
            {
                var timeStrings = item.Value;
                var studyTimes = TimeStringsToListStudyTime(timeStrings);
                scheduleTime[item.Key] = studyTimes;
            }
            var schedule = new Schedule(scheduleTime);

            IEnumerable<SupplementaryClassSchedule> supplementaryClassSchedules;
            var hasSupplementary = _tdTag.SelectSingleNode("//div[contains(@style, 'color: red; padding-top: 2px; text-align: center; position: relative')]");
            if (hasSupplementary != null)
            {
                var span = _tdTag.SelectSingleNode(@"//span[@class='content']");
                supplementaryClassSchedules = span.InnerHtml
                    .Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(GetSupplementaryClassSchedule);
                schedule.SupplementaryClassSchedules.AddRange(supplementaryClassSchedules);
            }
            return schedule;
        }

        /// <summary>
        /// Trả về array text được tách ra từ thẻ Td của thời gian học.
        /// </summary>
        /// <param name="trTag"></param>
        private static string[] ExtractDataFromTrTag(HtmlNode tdTag)
        {
            string[] separatingStrings = { " ", "\n", "\r", "-", "," };
            var trTagSplitDatas = tdTag.InnerText.Trim().Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries);
            return trTagSplitDatas;
        }

        /// <summary>
        /// Clean data time.
        /// </summary>
        /// <param name="tdTagSplitDatas"></param>
        /// <returns>Trả về array time item đã được clean.</returns>
        private static string[] CleanTimeItem(string[] tdTagSplitDatas)
        {
            var timeRegex = new Regex(@"^T[2-7]:|CN:|^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$");
            var times = tdTagSplitDatas.Where(item => timeRegex.IsMatch(item)).ToArray();
            return times;
        }

        /// <summary>
        /// Chuyển một chuỗi thời gian thành một List StudyTime.
        /// </summary>
        /// <param name="timeStrings">Time string phải match với pattern ^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$</param>
        /// <returns></returns>
        private static List<StudyTime> TimeStringsToListStudyTime(List<string> timeStrings)
        {
            if (timeStrings.Count % 2 == 1) throw new ArgumentException("Number of timeStrings must be even");

            //timeStrings = timeStrings.Distinct().ToList();
            var studyTimes = new List<StudyTime>();
            var i = 0;
            while (i < timeStrings.Count)
            {
                var studyTime = new StudyTime(timeStrings[i], timeStrings[i + 1]);
                studyTimes.Add(studyTime);
                i += 2;
            }
            return studyTimes;
        }

        /// <summary>
        /// Nhận vào một chuỗi đại diện cho một Thứ trong Tuần. Trả về một enum cho hôm đó.
        /// </summary>
        /// <param name="date">T2: T4: T5: T6: T7: CN:</param>
        /// <returns>Enum Week.</returns>
        private static DayOfWeek DateToDateWeek(string date)
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
                default:
                    return DayOfWeek.Sunday;
            }
        }
    }
}
