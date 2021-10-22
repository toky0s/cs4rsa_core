using System;
using System.Collections.Generic;

namespace Cs4rsaDatabaseService.Models
{
    public class Session
    {
        public int SessionId { get; set; }
        public string Name { get; set; }
        public DateTime SaveDate { get; set; }
        public string SemesterValue { get; set; }
        public string YearValue { get; set; }
        public List<SessionDetail> SessionDetails { get; set; }
    }
}
