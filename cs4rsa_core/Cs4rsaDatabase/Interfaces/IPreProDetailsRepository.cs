using cs4rsa_core.Cs4rsaDatabase.Models;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace cs4rsa_core.Cs4rsaDatabase.Interfaces
{
    public interface IPreProDetailsRepository : IGenericRepository<PreProDetail>
    {
        Task<List<PreProDetail>> GetPreProSubjectsByProgramSubjectId(string courseId);
    }
}