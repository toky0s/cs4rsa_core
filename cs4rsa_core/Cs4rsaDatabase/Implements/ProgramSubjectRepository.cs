using cs4rsa_core.Cs4rsaDatabase.DataProviders;
using cs4rsa_core.Cs4rsaDatabase.Interfaces;
using cs4rsa_core.Cs4rsaDatabase.Models;

using Microsoft.EntityFrameworkCore;

using System.Threading.Tasks;

namespace cs4rsa_core.Cs4rsaDatabase.Implements
{
    public class ProgramSubjectRepository : GenericRepository<ProgramSubject>, IProgramSubjectRepository
    {
        public ProgramSubjectRepository(Cs4rsaDbContext context) : base(context)
        {
        }

        public async Task<ProgramSubject> GetByCourseIdAsync(string CourseId)
        {
            return await _context.ProgramSubjects.FirstOrDefaultAsync(p => p.CourseId == CourseId);
        }

        public Task<ProgramSubject> GetBySubjectCode(string subjectCode)
        {
            return _context.ProgramSubjects.FirstOrDefaultAsync(p => p.SubjectCode.Equals(subjectCode));
        }
    }
}
