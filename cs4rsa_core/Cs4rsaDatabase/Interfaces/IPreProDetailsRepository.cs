using Cs4rsa.Cs4rsaDatabase.Models;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cs4rsa.Cs4rsaDatabase.Interfaces
{
    public interface IPreProDetailsRepository : IGenericRepository<PreProDetail>
    {
        Task<List<PreProDetail>> GetPreProSubjectsByProgramSubjectId(string courseId);
    }
}