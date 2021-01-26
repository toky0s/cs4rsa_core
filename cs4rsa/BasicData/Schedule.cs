using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace cs4rsa.BasicData
{
    //<sumary>Class này đại diện cho thời gian học của một SchoolClass trong một Tuần.</sumary>
    class Schedule
    {
        private Hashtable schedule;
        public string[] Week = {"T2", "T3", "T4", "T5", "T6", "T7", "CN"};

        public const string Monday = "T2";
        public const string Tuseday = "T3";
        public const string Webnesday = "T4";
        public const string Thurday = "T5";
        public const string Friday = "T6";
        public const string Saturday = "T7";
        public const string Sunday = "CN";
        
        public Schedule(Hashtable schedule)
        {
            this.schedule = schedule;
        }
        
        public List<string> getSchoolDays()
        {
            return null;
        }

        public List<DateTime> getStudyTimesAtDay(string scheduleDay)
        {
            return null;
        }
    }
}
