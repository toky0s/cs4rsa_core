using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.Helper
{
    /// <summary>
    /// Class này chứa những phương thức hữu ích.
    /// </summary>
    class Helper
    {
        public static string getTimeFromEpoch()
        {
            long fromUnixEpoch = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
            return fromUnixEpoch.ToString();
        }
    }

    class ScheduleHelper
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
