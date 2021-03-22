using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    class StringHelper
    {
        public static string CleanString(string text)
        {
            return text.Trim();
        }

        public static string[] SplitAndRemoveAllSpace(string text)
        {
            string[] separatingStrings = { " ", "\n", "\r" };
            return text.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string[] SplitAndRemoveNewLine(string text)
        {
            string[] separatingStrings = { "\n", "\r" };
            return text.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string SuperCleanString(string text)
        {
            string[] separatingStrings = { " ", "\n", "\r" };
            string[] sliceStrings = text.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries);
            string ouput = String.Join(" ", sliceStrings);
            return ouput;
        }
    }
}
