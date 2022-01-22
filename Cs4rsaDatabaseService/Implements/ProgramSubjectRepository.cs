using Cs4rsaDatabaseService.DataProviders;
using Cs4rsaDatabaseService.Interfaces;
using Cs4rsaDatabaseService.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Cs4rsaDatabaseService.Implements
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
    }
}
