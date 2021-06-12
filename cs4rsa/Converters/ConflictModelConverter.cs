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
            string resultTime = "";
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
                resultTime += day + "\n" + timeString;
            }
            return resultTime;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
