using Cs4rsa.Cs4rsaDatabase.Models;

using System.Threading.Tasks;

namespace Cs4rsa.Cs4rsaDatabase.Interfaces
{
    public interface IProgramSubjectRepository : IGenericRepository<DbProgramSubject>
    {
        Task<DbProgramSubject> GetByCourseIdAsync(string CourseId);

        Task<DbProgramSubject> GetBySubjectCode(string subjectCode);
    }
}
