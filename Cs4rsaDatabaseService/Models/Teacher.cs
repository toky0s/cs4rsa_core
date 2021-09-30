using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsaDatabaseService.Models
{
    public class Teacher
    {
        public int TeacherId { get; set; }
        public string Name { get; set; }
        public string Sex { get; set; }
        public string Place { get; set; }
        public string Degree { get; set; }
        public string WorkUnit { get; set; }
        public string Position { get; set; }
        public string Subject { get; set; }
        public string Form { get; set; }
    }
}
