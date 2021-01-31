using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.Helper
{
    /// <summary>
    /// Class này chứa những phương thức hữu ích.
    /// </summary>
    class Helper
    {
        public static string getTimeFromEpoch()
        {
            long fromUnixEpoch = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
            return fromUnixEpoch.ToString();
        }
    }
}
