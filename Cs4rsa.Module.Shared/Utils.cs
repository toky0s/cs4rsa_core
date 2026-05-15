using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsa.Module.Shared
{
    public class Utils
    {
        public static bool IsProduction()
        {
            #if DEBUG
            return false;
            #else
            return true;
            #endif
        }
        public static bool IsOverlap(DateTime start1, DateTime end1, DateTime start2, DateTime end2)
        {
            // Điều kiện giao nhau:
            // - Khoảng 1 bắt đầu trước khi khoảng 2 kết thúc
            // - Khoảng 2 bắt đầu trước khi khoảng 1 kết thúc
            // - Hoặc Start(1) = End(2), hoặc Start(2) = End(1)

            bool overlap = (start1 < end2 && start2 < end1)
                           || start1 == end2
                           || start2 == end1;

            return overlap;
        }
    }
}
