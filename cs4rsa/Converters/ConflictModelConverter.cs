using cs4rsa.BasicData;
using cs4rsa.Helpers;
using cs4rsa.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace cs4rsa.Converters
{
    /// <summary>
    /// Chuyển một Conflict Model thành một chuỗi thông tin hiển thị.
    /// </summary>
    class ConflictModelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ConflictTime conflictTime = value as ConflictTime;
            List<string> resultTimes = new List<string>();
            foreach (KeyValuePair<DayOfWeek, List<StudyTimeIntersect>> item in conflictTime.ConflictTimes)
            {
                string day = BasicDataConverter.ToDayOfWeekText(item.Key);
                List<string> times = new List<string>();
                foreach (StudyTimeIntersect studyTimeIntersect in item.Value)
                {
                    string time = $"Từ {studyTimeIntersect.Start} đến {studyTimeIntersect.End}";
                    times.Add(time);
                }
                string timeString = string.Join("\n", times);
                resultTimes.Add(day + "\n" + timeString);
            }
            return string.Join("\n", resultTimes);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
