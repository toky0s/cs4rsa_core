using System.Collections.Generic;

namespace Cs4rsa.Services.ProgramSubjectCrawlerSvc.DataTypes
{
    public class PlanTable
    {
        public string Name { get; set; }
        public List<PlanRecord> PlanRecords { get; set; }
        public int TotalUnit { get; set; }
    }
}
