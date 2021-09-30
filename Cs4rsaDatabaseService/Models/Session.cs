using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsaDatabaseService.Models
{
    public class Session
    {
        public int SessionId { get; set; }
        public string Name { get; set; }
        public DateTime SaveDate { get; set; }
        public int SemesterValue { get; set; }
        public int YearValue { get; set; }
        public List<SessionDetail> SessionDetails { get; set; }
    }
}
