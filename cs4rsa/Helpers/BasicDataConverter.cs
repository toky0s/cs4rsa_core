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
            if (slideds.Contains("Hoà"))
                return Place.HOAKHANH;
            if (slideds.Contains("254"))
                return Place.PHANTHANH;
            else
                return Place.VIETTIN;
        }
    }
}
