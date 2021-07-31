using cs4rsa.Crawler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.Models
{
    /// <summary>
    /// Class này đại diện cho một Session được lưu trong database.
    /// </summary>
    class ScheduleSession
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public DateTime SaveDate { get; set; }
        public string Semester { get; set; }
        public string Year { get; set; }

        public bool IsValid()
        {
            HomeCourseSearch hcs = HomeCourseSearch.GetInstance();
            if (Semester.Equals(hcs.CurrentSemesterValue) && Year.Equals(hcs.CurrentYearValue))
                return true;
            return false;
        }
    }
}
