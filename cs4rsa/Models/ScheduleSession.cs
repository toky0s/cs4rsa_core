using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.Models
{
    class ScheduleSession
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public DateTime SaveDate { get; set; }
        public string Semester { get; set; }
        public string Year { get; set; }
    }
}
