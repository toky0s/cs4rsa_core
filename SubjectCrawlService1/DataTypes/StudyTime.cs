using SubjectCrawlService1.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubjectCrawlService1.DataTypes
{
    public class StudyTime
    {
        private readonly DateTime _start;
        private readonly DateTime _end;
        public DateTime Start { get { return _start; } }
        public DateTime End { get { return _end; } }

        private readonly string _startAsString;
        private readonly string _endAsString;
        public string StartAsString => _startAsString;
        public string EndAsString => _endAsString;

        public StudyTime()
        {

        }

        public StudyTime(DateTime start, DateTime end)
        {
            _start = start;
            _end = end;
        }

        public StudyTime(string start, string end)
        {
            _startAsString = start;
            _endAsString = end;
            _start = DateTime.ParseExact(start, "HH:mm", System.Globalization.CultureInfo.InvariantCulture);
            _end = DateTime.ParseExact(end, "HH:mm", System.Globalization.CultureInfo.InvariantCulture);
        }

        public double TotalHours()
        {
            return (_end - _start).TotalHours;
        }

        public Session GetSession()
        {
            if (!IsInMorning())
            {
                if (!IsInAfternoon())
                {
                    return Session.Night;
                }
                return Session.Afternoon;
            }
            return Session.Morning;
        }


        /// <summary>
        /// Kiểm tra đây có phải là buổi sáng hay không.
        /// </summary>
        /// <returns></returns>
        private bool IsInMorning()
        {
            DateTime[] MorningTime =  {
                DateTime.ParseExact("07:00", "HH:mm", System.Globalization.CultureInfo.InvariantCulture),
                DateTime.ParseExact("11:15", "HH:mm", System.Globalization.CultureInfo.InvariantCulture)
            };
            if (_start <= MorningTime[1])
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Kiểm tra đây có phải là buổi chiều hay không.
        /// </summary>
        /// <returns></returns>
        private bool IsInAfternoon()
        {
            DateTime[] AfternoonTime =  {
                DateTime.ParseExact("13:00", "HH:mm", System.Globalization.CultureInfo.InvariantCulture),
                DateTime.ParseExact("17:15", "HH:mm", System.Globalization.CultureInfo.InvariantCulture)
            };
            if (_start >= AfternoonTime[0] && _start <= AfternoonTime[1])
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Kiểm tra đây có phải là buổi tối hay không.
        /// </summary>
        /// <returns></returns>
        private bool IsInNight()
        {
            DateTime[] NightTime =  {
                DateTime.ParseExact("17:45", "HH:mm", System.Globalization.CultureInfo.InvariantCulture),
                DateTime.ParseExact("21:00", "HH:mm", System.Globalization.CultureInfo.InvariantCulture)
            };
            if (_start >= NightTime[0] && _end <= NightTime[1])
            {
                return true;
            }
            return false;
        }
    }
}
