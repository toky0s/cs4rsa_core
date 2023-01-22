using Cs4rsa.Services.ProgramSubjectCrawlerSvc.DataTypes;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cs4rsa.Services.ProgramSubjectCrawlerSvc.Interfaces
{
    public interface IStudentPlanCrawler
    {
        Task<List<PlanTable>> GetPlanTables(int curriculumId, string sessionId);
        Task<List<PlanTable>> GetPlanTables(int curriculumId);
    }
}
