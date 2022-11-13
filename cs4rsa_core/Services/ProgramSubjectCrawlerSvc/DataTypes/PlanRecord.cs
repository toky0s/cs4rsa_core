using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa_core.Services.ProgramSubjectCrawlerSvc.DataTypes
{
    public class PlanRecord
    {
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public string Url { get; set; }
        public int StudyUnit { get; set; }
    }
}
