using Cs4rsa.Cs4rsaDatabase.Models;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cs4rsa.Cs4rsaDatabase.Interfaces
{
    public interface IProgramSubjectRepository : IGenericRepository<DbProgramSubject>
    {
        Task<bool> ExistsByCourseId(string courseId);
        IEnumerable<string> GetParByCourseId(string courseId);
        IEnumerable<string> GetPreByCourseId(string courseId);
    }
}
