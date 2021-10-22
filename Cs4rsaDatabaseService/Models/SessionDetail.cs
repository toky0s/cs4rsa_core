using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsaDatabaseService.Models
{
    public class SessionDetail
    {
        public int SessionDetailId { get; set; }
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public string ClassGroup { get; set; }
        public int SessionId { get; set; }
        public Session Session { get; set; }
    }
}
