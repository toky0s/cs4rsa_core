using Cs4rsa.Utils.Models;

using System;

namespace Cs4rsa.Utils
{
    public static class DayOfWeekExtension
    {
        public static string ToCs4rsaThu(this DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Sunday => "Chủ Nhật",
                DayOfWeek.Monday => "Thứ Hai",
                DayOfWeek.Tuesday => "Thứ Ba",
                DayOfWeek.Wednesday => "Thứ Tư",
                DayOfWeek.Thursday => "Thứ Năm",
                DayOfWeek.Friday => "Thứ Sáu",
                _ => "Thứ Bảy",
            };
        }

        public static int ToIndex(this DayOfWeek dayOfWeek)
        {
            return dayOfWeek == DayOfWeek.Sunday ? 6 : (int)dayOfWeek - 1;
        }
    }
}
