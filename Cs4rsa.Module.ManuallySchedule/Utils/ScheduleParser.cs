using Cs4rsa.Constants;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes;

using HtmlAgilityPack;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Cs4rsa.Services.SubjectCrawlerSvc.Utils
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
            Dictionary<DayOfWeek, List<StudyTime>> scheduleTime = new();
            Regex regexDate = new(@"^T[2-7]:$|^CN:$");
            Regex regexTime = new(@"^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$");

            int i = 0;
            DayOfWeek dayOfWeek = DayOfWeek.Sunday;
            Dictionary<DayOfWeek, List<string>> dateTimeStringPairs = new();
            while (i < times.Length)
            {
                if (regexDate.IsMatch(times[i]))
                {
                    dayOfWeek = DateToDateWeek(times[i]);
                    List<string> timeStrings = new();
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
        private static string[] ExtractDataFromTrTag(HtmlNode tdTag)
        {
            string[] separatingStrings = { VmConstants.StrSpace, "\n", "\r", "-", "," };
            string[] trTagSplitDatas = tdTag.InnerText.Trim().Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries);
            return trTagSplitDatas;
        }

        /// <summary>
        /// Clean data time.
        /// </summary>
        /// <param name="tdTagSplitDatas"></param>
        /// <returns>Trả về array time item đã được clean.</returns>
        private static string[] CleanTimeItem(string[] tdTagSplitDatas)
        {
            Regex timeRegex = new(@"^T[2-7]:|CN:|^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$");
            string[] times = tdTagSplitDatas.Where(item => timeRegex.IsMatch(item)).ToArray();
            return times;
        }

        /// <summary>
        /// Chuyển một chuỗi thời gian thành một List StudyTime.
        /// </summary>
        /// <param name="timeStrings">Time string phải match với pattern ^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$</param>
        /// <returns></returns>
        private static List<StudyTime> TimeStringsToListStudyTime(List<string> timeStrings)
        {
            timeStrings = timeStrings.Distinct().ToList();
            List<StudyTime> studyTimes = new();
            int i = 0;
            while (i < timeStrings.Count)
            {
                StudyTime studyTime = new(timeStrings[i], timeStrings[i + 1]);
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
            return date switch
            {
                "T2:" => DayOfWeek.Monday,
                "T3:" => DayOfWeek.Tuesday,
                "T4:" => DayOfWeek.Wednesday,
                "T5:" => DayOfWeek.Thursday,
                "T6:" => DayOfWeek.Friday,
                "T7:" => DayOfWeek.Saturday,
                _ => DayOfWeek.Sunday,
            };
        }
    }
}
