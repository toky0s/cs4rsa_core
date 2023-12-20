using Cs4rsa.Service.SubjectCrawler.DataTypes.Enums;

using System;
using System.Linq;

namespace Cs4rsa.Services.SubjectCrawlerSvc.Utils
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
            string[] slideds = place.Split(' ');
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
            return StudyUnitType.Credit;
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
            return ImplementType.NotStartYet;
        }

        public static string ToDayOfWeekText(this DayOfWeek day)
        {
            if (day == DayOfWeek.Sunday)
                return "Chủ Nhật";
            return $"Thứ {(int)day + 1}";
        }
    }
}
