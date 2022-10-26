using System;

namespace cs4rsa_core.Utils
{
    /// <summary>
    /// Class này chứa những phương thức hữu ích.
    /// </summary>
    public class Helpers
    {
        public static string GetTimeFromEpoch()
        {
            long fromUnixEpoch = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
            return fromUnixEpoch.ToString();
        }

        public static string GetFilePathAtApp(string fileName)
        {
            return AppDomain.CurrentDomain.BaseDirectory + @"\" + fileName;
        }
    }
}
