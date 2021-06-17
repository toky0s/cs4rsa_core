using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.BasicData;

namespace cs4rsa.Helpers
{
    /// <summary>
    /// Class này chứa những phương thức hữu ích.
    /// </summary>
    public class Helpers
    {
        public static string GetTimeFromEpoch()
        {
            long fromUnixEpoch = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
            return fromUnixEpoch.ToString();
        }

        public static string GetFilePathAtApp(string fileName)
        {
            return AppDomain.CurrentDomain.BaseDirectory + @"\" + fileName;
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

    public class Checker
    {
        /// <summary>
        /// Kiểm tra một List<T> có 
        /// </summary>
        /// <typeparam name="T">Là một trong các BasicData</typeparam>
        /// <param name="thisSet"></param>
        /// <param name="thatSet"></param>
        /// <returns></returns>
        public static bool ThisSetInThatSet<T>(List<T> thisSet, List<T> thatSet)
        {
            List<T> outSet = thisSet.Intersect(thatSet).ToList();
            if (outSet.Count >= thatSet.Count)
            {
                return true;
            }
            return false;
        }
    }
}
