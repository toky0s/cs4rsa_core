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
        private string[] Week = { "T2", "T3", "T4", "T5", "T6", "T7", "CN" };

        public string Monday = "T2";
        public string Tuseday = "T3";
        public string Webnesday = "T4";
        public string Thurday = "T5";
        public string Friday = "T6";
        public string Saturday = "T7";
        public string Sunday = "CN";
        
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
