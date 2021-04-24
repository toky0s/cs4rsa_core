using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.Helpers
{
    public class ShortedTime
    {
        public readonly DateTime RawTime;
        public readonly DateTime NewTime;
        public ShortedTime(DateTime raw, DateTime converted)
        {
            RawTime = raw;
            NewTime = converted;
        }
    }

    public class ShortedTimeConverter
    {
        private readonly DateTime now = DateTime.Now;
        private readonly Dictionary<DateTime, DateTime> DuyTanStudyTimes = new Dictionary<DateTime, DateTime>();

        public ShortedTimeConverter()
        {
            AddDuyTanTime(7, 15, 7, 0);
            AddDuyTanTime(9, 15, 9, 0);
            AddDuyTanTime(10, 15, 10, 0);
            AddDuyTanTime(11, 15, 11, 0);
            AddDuyTanTime(15, 15, 15, 0);
            AddDuyTanTime(16, 15, 16, 0);
            AddDuyTanTime(17, 15, 17, 0);
            AddDuyTanTime(17, 45, 18, 0);
        }

        public ShortedTime Convert(DateTime time)
        {
            DateTime converted = DuyTanStudyTimes[time];
            if (converted != null)
                return new ShortedTime(time, converted);
            return new ShortedTime(time, time);
        }

        private void AddDuyTanTime(int rawHour, int rawMinute, int newHour, int newMinute)
        {
            DateTime now = DateTime.Now;
            DuyTanStudyTimes.Add(
                new DateTime(now.Year, now.Month, now.Day, rawHour, rawMinute, 0), 
                new DateTime(now.Year, now.Month, now.Day, newHour, newMinute, 0)
            );
        }
    }
}
