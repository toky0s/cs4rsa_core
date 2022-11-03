using cs4rsa_core.Services.SubjectCrawlerSvc.DataTypes.Enums;

using System;
using System.Linq;

namespace cs4rsa_core.Services.SubjectCrawlerSvc.Utils
{
    /// <summary>
    /// Chịu trách nhiệm chuyển đổi các kiểu dữ liệu thông thường sang các BasicData hoặc Model
    /// và ngược lại.
    /// </summary>
    public static class BasicDataConverter
    {
        /// <summary>
        /// Chuyển một chuỗi về Place. Xác định nơi học của một lớp học.
        /// </summary>
        /// <param name="place"></param>
        /// <returns></returns>
        public static Place ToPlace(string place)
        {
            char[] splitChars = { ' ' };
            string[] slideds = place.Split(splitChars);
            if (slideds.Contains("Quang"))
                return Place.QUANGTRUNG;
            if (slideds.Contains("Nam"))
                return Place.HOAKHANH;
            if (slideds.Contains("209"))
                return Place.PHANTHANH;
            if (slideds.Contains("334/4"))
                return Place.VIETTIN;
            if (slideds.Contains("254"))
                return Place.NVL_254;
            if (slideds.Contains("137"))
                return Place.NVL_137;
            return Place.ONLINE;
        }

        public static Phase ToPhase(string phase)
        {
            if (phase == "both") return Phase.All;
            if (phase == "first") return Phase.First;
            if (phase == "second") return Phase.Second;
            return Phase.Non;
        }

        public static DayOfWeek ToDayOfWeek(string day)
        {
            if (day.Contains('2') || day.Contains("Hai") || day.Contains("hai")) return DayOfWeek.Monday;
            if (day.Contains('3') || day.Contains("Ba") || day.Contains("ba")) return DayOfWeek.Tuesday;
            if (day.Contains('4') || day.Contains("Tư") || day.Contains("tư")) return DayOfWeek.Wednesday;
            if (day.Contains('5') || day.Contains("Năm") || day.Contains("năm")) return DayOfWeek.Thursday;
            if (day.Contains('6') || day.Contains("Sáu") || day.Contains("sáu")) return DayOfWeek.Friday;
            if (day.Contains('7') || day.Contains("Bảy") || day.Contains("bảy")) return DayOfWeek.Saturday;
            return DayOfWeek.Sunday;
        }

        public static StudyUnitType ToStudyUnitType(string text)
        {
            return text switch
            {
                _ => StudyUnitType.Credit,
            };
        }

        public static RegistrationType ToRegistrationType(string text)
        {
            if (text.Equals("Còn Hạn Đăng Ký"))
                return RegistrationType.Valid;
            return RegistrationType.Expired;
        }

        public static ImplementType ToImplementType(string text)
        {
            if (text.Equals("Lớp Học Đã Bị Hủy"))
                return ImplementType.Canceled;
            if (text.Equals("Lớp Học Đã Bắt Đầu"))
                return ImplementType.Started;
            return ImplementType.Unstart;
        }

        public static string ToDayOfWeekText(this DayOfWeek day)
        {
            if (day == DayOfWeek.Sunday)
                return "Chủ Nhật";
            return $"Thứ {(int)day + 1}";
        }
    }
}
