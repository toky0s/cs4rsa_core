using System;

namespace cs4rsa_core.Utils
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
    }
}
