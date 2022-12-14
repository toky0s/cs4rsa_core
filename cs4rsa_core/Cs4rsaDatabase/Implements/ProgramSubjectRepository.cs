using Cs4rsa.Cs4rsaDatabase.DataProviders;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;

using Microsoft.EntityFrameworkCore;

using System.Threading.Tasks;

namespace Cs4rsa.Cs4rsaDatabase.Implements
{
    public class ProgramSubjectRepository : GenericRepository<DbProgramSubject>, IProgramSubjectRepository
    {
        public ProgramSubjectRepository(Cs4rsaDbContext context) : base(context)
        {
        }

        public async Task<DbProgramSubject> GetByCourseIdAsync(string CourseId)
        {
            return await _context.DbProgramSubjects.FirstOrDefaultAsync(p => p.CourseId == CourseId);
        }

        public Task<DbProgramSubject> GetBySubjectCode(string subjectCode)
        {
            return _context.DbProgramSubjects.FirstOrDefaultAsync(p => p.SubjectCode.Equals(subjectCode));
        }
    }
}
