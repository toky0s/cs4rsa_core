using System;
using System.Collections.Generic;

namespace HelperService
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
    /// Bộ chuyển đổi này sẽ chuyển một số mốc thời gian lẻ cố định như 07:15 thành
    /// 07:00 để tiện cho các sinh viên theo dõi.
    /// </summary>
    public class ShortedTimeConverter
    {
        private static ShortedTimeConverter _instance = new();
        private readonly Dictionary<DateTime, DateTime> DuyTanStudyTimes = new();

        private ShortedTimeConverter()
        {
        }

        public static ShortedTimeConverter GetInstance()
        {
            return _instance;
        }

        public ShortedTime Convert(DateTime time)
        {
            if (!DuyTanStudyTimes.ContainsKey(time))
            {
                return new ShortedTime(time, time);
            }
            DateTime converted = DuyTanStudyTimes[time];
            return new ShortedTime(time, converted);
        }
    }
}
