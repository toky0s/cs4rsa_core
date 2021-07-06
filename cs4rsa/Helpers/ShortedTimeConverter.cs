using cs4rsa.BasicData;
using System;
using System.Collections.Generic;

namespace cs4rsa.Helpers
{
    public class ShortedTime: IComparable<ShortedTime>
    {
        public readonly DateTime RawTime;
        public readonly DateTime NewTime;
        public ShortedTime(DateTime raw, DateTime converted)
        {
            RawTime = raw;
            NewTime = converted;
        }

        public int CompareTo(ShortedTime other)
        {
            return RawTime.CompareTo(other.RawTime);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ShortedTime shortedTime))
            {
                return false;
            }
            return RawTime == shortedTime.RawTime;
        }

        public override int GetHashCode()
        {
            return this.RawTime.GetHashCode();
        }

        public static bool operator >=(ShortedTime left, ShortedTime right)
        {
            return left.RawTime > right.RawTime || left.RawTime == right.RawTime;
        }

        public static bool operator <=(ShortedTime left, ShortedTime right)
        {
            return left.RawTime < right.RawTime || left.RawTime == right.RawTime;
        }
    }


    /// <summary>
    /// Impelement Singleton.
    /// </summary>
    public class ShortedTimeConverter
    {
        private static ShortedTimeConverter _instance = new ShortedTimeConverter();
        private readonly DateTime now = DateTime.Now;
        private readonly Dictionary<DateTime, DateTime> DuyTanStudyTimes = new Dictionary<DateTime, DateTime>();

        private ShortedTimeConverter()
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

        public static ShortedTimeConverter GetInstance()
        {
            return _instance;
        }

        public ShortedTime Convert(DateTime time)
        {
            if (!DuyTanStudyTimes.ContainsKey(time))
                return new ShortedTime(time, time);
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

        public List<ShortedTime> ToShortedTime(List<StudyTime> studyTimes)
        {
            List<ShortedTime> shortedTimes = new List<ShortedTime>();
            foreach (StudyTime studyTime in studyTimes)
            {
                ShortedTime shortedTimeStart = Convert(studyTime.Start);
                if (!shortedTimes.Contains(shortedTimeStart))
                    shortedTimes.Add(shortedTimeStart);

                ShortedTime shortedTimeEnd = Convert(studyTime.End);
                if (!shortedTimes.Contains(shortedTimeEnd))
                    shortedTimes.Add(shortedTimeEnd);
            }
            return shortedTimes;
        }
    }
}
