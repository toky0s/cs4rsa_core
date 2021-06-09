using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.BasicData;
using cs4rsa.Helpers;

namespace cs4rsa.Helpers
{
    /// <summary>
    /// Chịu trách nhiệm chuyển đổi các kiểu dữ liệu thông thường sang các BasicData
    /// </summary>
    public class BasicDataConverter
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
            if (slideds.Contains("03"))
                return Place.QUANGTRUNG;
            if (slideds.Contains("Nam"))
                return Place.HOAKHANH;
            if (slideds.Contains("209") || slideds.Contains("254"))
                return Place.PHANTHANH;
            if (slideds.Contains("334/4"))
                return Place.VIETTIN;
            else
                return Place.NVL_137;
        }

        public static Phase ToPhase(string phase)
        {
            if (phase == "both") return Phase.ALL;
            if (phase == "first") return Phase.FIRST;
            if (phase == "second") return Phase.SECOND;
            return Phase.NON;
        }

        public static DayOfWeek ToDayOfWeek(string day)
        {
            if (day.Contains("2") || day.Contains("Hai") || day.Contains("hai")) return DayOfWeek.Monday;
            if (day.Contains("3") || day.Contains("Ba") || day.Contains("ba")) return DayOfWeek.Tuesday;
            if (day.Contains("4") || day.Contains("Tư") || day.Contains("tư")) return DayOfWeek.Wednesday;
            if (day.Contains("5") || day.Contains("Năm") || day.Contains("năm")) return DayOfWeek.Thursday;
            if (day.Contains("6") || day.Contains("Sáu") || day.Contains("sáu")) return DayOfWeek.Friday;
            if (day.Contains("7") || day.Contains("Bảy") || day.Contains("bảy")) return DayOfWeek.Saturday;
            return DayOfWeek.Sunday;
        }
    }
}
