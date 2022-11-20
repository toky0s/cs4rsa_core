using Cs4rsa.Cs4rsaDatabase.Models;

using System.Threading.Tasks;

namespace Cs4rsa.Cs4rsaDatabase.Interfaces
{
    public interface IProgramSubjectRepository : IGenericRepository<ProgramSubject>
    {
        Task<ProgramSubject> GetByCourseIdAsync(string CourseId);

        Task<ProgramSubject> GetBySubjectCode(string subjectCode);
    }
}
