using Cs4rsa.Cs4rsaDatabase.DataProviders;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;

using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cs4rsa.Cs4rsaDatabase.Implements
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
            DbProgramSubject programSubject = await _context.DbProgramSubjects.Where(pp => pp.CourseId == courseId).FirstOrDefaultAsync();
            return await _context.PreProDetails
                .Where(pp => pp.ProgramSubjectId == programSubject.DbProgramSubjectId)
                .ToListAsync();
        }
    }
}
