using Cs4rsaDatabaseService.Models;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cs4rsaDatabaseService.Interfaces
{
    public interface IPreProDetailsRepository : IGenericRepository<PreProDetail>
    {
        Task<List<PreProDetail>> GetPreProSubjectsByProgramSubjectId(string courseId);
    }
}