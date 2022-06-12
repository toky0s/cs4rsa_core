using Cs4rsaDatabaseService.Models;

using System.Threading.Tasks;

namespace Cs4rsaDatabaseService.Interfaces
{
    public interface IProgramSubjectRepository : IGenericRepository<ProgramSubject>
    {
        Task<ProgramSubject> GetByCourseIdAsync(string CourseId);

        Task<ProgramSubject> GetBySubjectCode(string subjectCode);
    }
}
