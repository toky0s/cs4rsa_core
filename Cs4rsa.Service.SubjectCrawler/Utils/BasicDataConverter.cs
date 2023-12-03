using System;
using System.Linq;
using Cs4rsa.Service.SubjectCrawler.DataTypes.Enums;

namespace Cs4rsa.Service.SubjectCrawler.Utils
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
            var slideds = place.Split(' ');
            if (slideds.Contains("Quang"))
                return Place.QuangTrung;
            if (slideds.Contains("Nam"))
                return Place.HoaKhanh;
            if (slideds.Contains("209"))
                return Place.PhanThanh;
            if (slideds.Contains("334/4"))
                return Place.VietTin;
            if (slideds.Contains("254"))
                return Place.Nvl254;
            if (slideds.Contains("137"))
                return Place.Nvl137;
            return Place.Online;
        }

        public static StudyUnitType ToStudyUnitType(string text)
        {
            switch (text)
            {
                default:
                    return StudyUnitType.Credit;
            }
        }

        public static RegistrationType ToRegistrationType(string text)
        {
            return text.Equals("Còn Hạn Đăng Ký") ? RegistrationType.Valid : RegistrationType.Expired;
        }

        public static ImplementType ToImplementType(string text)
        {
            switch (text)
            {
                case "Lớp Học Đã Bị Hủy":
                    return ImplementType.Canceled;
                case "Lớp Học Đã Bắt Đầu":
                    return ImplementType.Started;
                default:
                    return ImplementType.NotStartYet;
            }
        }

        public static string ToDayOfWeekText(this DayOfWeek day)
        {
            return day == DayOfWeek.Sunday ? "Chủ Nhật" : $"Thứ {(int)day + 1}";
        }
        
        public static string ToCs4rsaThu(this DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return "Chủ Nhật";
                case DayOfWeek.Monday:
                    return "Thứ Hai";
                case DayOfWeek.Tuesday:
                    return "Thứ Ba";
                case DayOfWeek.Wednesday:
                    return "Thứ Tư";
                case DayOfWeek.Thursday:
                    return "Thứ Năm";
                case DayOfWeek.Friday:
                    return "Thứ Sáu";
                case DayOfWeek.Saturday:
                default:
                    return "Thứ Bảy";
            }
        }
    }
}
