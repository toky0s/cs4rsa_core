using Cs4rsa.Cs4rsaDatabase.DataProviders;
using Cs4rsa.Cs4rsaDatabase.DTOs;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;

using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public List<DtoDbProgramSubject> GetDbProgramSubjectsByCrrId(int crrId)
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine("    dps.SubjectCode");
            sql.AppendLine("  , dps.Name");
            sql.AppendLine("  , dps.Credit");
            sql.AppendLine("  , kw.CourseId");
            sql.AppendLine("  , kw.Color");
            sql.AppendLine("  , kw.Cache");
            sql.AppendLine("FROM DbProgramSubjects AS dps");
            sql.AppendLine("LEFT JOIN Keywords AS kw");
            sql.AppendLine("	ON kw.CourseId = dps.CourseId");
            sql.AppendLine("WHERE dps.CurriculumId = @CurriculumId");
            IDictionary<string, object> param = new Dictionary<string, object>()
            {
                { "@CurriculumId", crrId }
            };
            return _rawSql.ExecReader(
                  sql.ToString()
                , param
                , record => new DtoDbProgramSubject()
                {
                    SubjectCode = record.GetString(0),
                    Name = record.GetString(1),
                    Credit = record.GetInt32(2),
                    CourseId = record.IsDBNull(3) ? null : record.GetInt32(3).ToString(),
                    Color = record.IsDBNull(4) ? null : record.GetString(4),
                    Cache = record.IsDBNull(5) ? null : record.GetString(5),
                }
            ).ToList();
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
