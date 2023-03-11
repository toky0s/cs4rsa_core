using Cs4rsa.Cs4rsaDatabase.DataProviders;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;

using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cs4rsa.Cs4rsaDatabase.Implements
{
    public class ProgramSubjectRepository : GenericRepository<DbProgramSubject>, IProgramSubjectRepository
    {
        public ProgramSubjectRepository(Cs4rsaDbContext context) : base(context)
        {
        }

        public async Task<bool> ExistsByCourseId(string courseId)
        {
            return await _context.DbProgramSubjects.AnyAsync(ps => ps.CourseId.Equals(courseId));
        }

        public IEnumerable<string> GetParByCourseId(string courseId)
        {
            return from p in _context.DbProgramSubjects
                   join d in _context.ParProDetails on p.DbProgramSubjectId equals d.ProgramSubjectId
                   join a in _context.DbPreParSubjects on d.PreParSubjectId equals a.DbPreParSubjectId
                   select a.SubjectCode;
        }

        public IEnumerable<string> GetPreByCourseId(string courseId)
        {
            return from p in _context.DbProgramSubjects
                   join d in _context.PreProDetails on p.DbProgramSubjectId equals d.ProgramSubjectId
                   join a in _context.DbPreParSubjects on d.PreParSubjectId equals a.DbPreParSubjectId
                   select a.SubjectCode;
        }
    }
}
