using cs4rsa_core.Services.ProgramSubjectCrawlerSvc.DataTypes;
using cs4rsa_core.Utils.Interfaces;
using cs4rsa_core.Utils;

using HtmlAgilityPack;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace cs4rsa_core.Services.ProgramSubjectCrawlerSvc.Interfaces
{
    public interface IStudentPlanCrawler
    {
        Task<IEnumerable<PlanTable>> GetPlanTables(int curriculumId, string sessionId);
        Task<IEnumerable<PlanTable>> GetPlanTables(int curriculumId);
    }
}
