using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.BasicData
{
    class StudyTime
    {
        private DateTime start;
        private DateTime end;
        public DateTime Start {get { return start; } set { start = value; }}
        public DateTime End { get { return end; } set { end = value; } }

        public StudyTime()
        {

        }

        public StudyTime(DateTime start, DateTime end)
        {
            this.start = start;
            this.end = end;
        }

        public StudyTime(string start, string end)
        {
            this.start = DateTime.ParseExact(start, "HH:mm", System.Globalization.CultureInfo.InvariantCulture);
            this.end = DateTime.ParseExact(end, "HH:mm", System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
