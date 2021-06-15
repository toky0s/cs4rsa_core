using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.BasicData
{

    /// <summary>
    /// Trình tìm kiếm xung đột hai giờ đầu và hai giờ cuối của hai ClassGroup
    /// </summary>
    class PlaceConflictFinder
    {
        private static readonly List<BetweenPoint> BetweenPointCollection = new List<BetweenPoint>();
        private readonly ClassGroup _classGroup1;
        private readonly ClassGroup _classGroup2;

        public PlaceConflictFinder(ClassGroup classGroup1, ClassGroup classGroup2)
        {
            // init between point collection

            _classGroup1 = classGroup1;
            _classGroup2 = classGroup2;
        }
    }
}
