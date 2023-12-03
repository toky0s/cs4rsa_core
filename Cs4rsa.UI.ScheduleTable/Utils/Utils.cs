﻿using Cs4rsa.Constants;

using System;
using System.Globalization;

namespace Cs4rsa.Controls
{
    public class Utils
    {
        public static readonly string[] TIME_LINES = new string[17]
        {
            "07:00",
            "08:00",
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
            string time = dateTime.ToString(VmConstants.TimeFormat, CultureInfo.CurrentCulture);
            return Array.IndexOf(TIME_LINES, time);
        }
    }
}
