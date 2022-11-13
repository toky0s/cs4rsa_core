using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa_core.Services.ProgramSubjectCrawlerSvc.DataTypes
{
    public class PlanTable
    {
        public string Name { get; set; }
        public IEnumerable<PlanRecord> PlanRecords { get; set; }
    }
}
