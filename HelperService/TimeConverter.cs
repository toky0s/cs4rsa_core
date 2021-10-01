using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperService
{
    public class TimeConverter
    {
        /// <summary>
        /// Chuỗi thời gian theo format HH:mm
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime GetDateTimeFromString(string time)
        {
            return DateTime.ParseExact(time, "HH:mm", System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
