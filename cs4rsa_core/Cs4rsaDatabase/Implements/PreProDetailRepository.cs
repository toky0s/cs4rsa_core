using cs4rsa_core.Cs4rsaDatabase.DataProviders;
using cs4rsa_core.Cs4rsaDatabase.Interfaces;
using cs4rsa_core.Cs4rsaDatabase.Models;

using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cs4rsa_core.Cs4rsaDatabase.Implements
{
    public class PreProDetailRepository : GenericRepository<PreProDetail>, IPreProDetailsRepository
    {
        public PreProDetailRepository(Cs4rsaDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Lấy ra danh sách thông tin các môn tiên quyết của một môn theo Course ID
        /// </summary>
        /// <param name="courseId">Course ID</param>
        /// <returns>Danh sách thông tin môn tiên quyết</returns>
        public async Task<List<PreProDetail>> GetPreProSubjectsByProgramSubjectId(string courseId)
        {
            ProgramSubject programSubject = await _context.ProgramSubjects.Where(pp => pp.CourseId == courseId).FirstOrDefaultAsync();
            return await _context.PreProDetails
                .Where(pp => pp.ProgramSubjectId == programSubject.ProgramSubjectId)
                .ToListAsync();
        }
    }
}
