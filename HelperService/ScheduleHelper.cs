using System;

namespace HelperService
{
    public class ScheduleHelper
    {
        /// <summary>
        /// Chuyển một chuỗi thời gian match với đoạn regex sau @"^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$"
        /// thành kiểu DateTime.
        /// </summary>
        /// <param name="time">Chuỗi thời gian, ví dụ: 15:30.</param>
        /// <returns>DateTime.</returns>
        public static DateTime TimeStringToDateTime(string time)
        {
            return DateTime.ParseExact(time, "HH:mm", System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
