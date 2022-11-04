using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa_core.Controls
{
    internal class Utils
    {
        public static readonly string[] TIME_LINES = new string[16]
        {
            "07:00",
            "09:00",
            "09:15",
            "10:15",
            "11:15",
            "12:00",
            "13:00",
            "14:00",
            "15:00",
            "15:15",
            "16:15",
            "17:15",
            "17:45",
            "18:45",
            "19:45",
            "21:00"
        };

        public static int GetTimeIndex(DateTime dateTime)
        {
            string time = dateTime.ToString("HH:mm", CultureInfo.CurrentCulture);
            return Array.IndexOf(TIME_LINES, time);
        }
    }
}
