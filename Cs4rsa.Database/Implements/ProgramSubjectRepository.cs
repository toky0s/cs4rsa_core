using System.Collections.Generic;
using System.Text;
using Cs4rsa.Database.DataProviders;
using Cs4rsa.Database.DTOs;
using Cs4rsa.Database.Interfaces;
using Cs4rsa.Database.Models;

namespace Cs4rsa.Database.Implements
{
    public class ProgramSubjectRepository : IProgramSubjectRepository
    {
        private readonly RawSql _rawSql;

        public ProgramSubjectRepository(RawSql rawSql)
        {
            _rawSql = rawSql;
        }
        public void Add(DbProgramSubject programSubject)
        {
            var dbProgramSubjectId = _rawSql.ExecScalar("SELECT COUNT(*) + 1 FROM DbProgramSubjects", 0L);
            var sb = new StringBuilder()
                .AppendLine("INSERT INTO DbProgramSubjects VALUES")
                .AppendLine("(")
                .AppendLine("  @DbProgramSubjectId")
                .AppendLine(", @SubjectCode")
                .AppendLine(", @CourseId")
                .AppendLine(", @Name")
                .AppendLine(", @Credit")
                .AppendLine(", @CurriculumId")
                .AppendLine(")");

            var param = new Dictionary<string, object>
            {
                { "@DbProgramSubjectId", dbProgramSubjectId},
                { "@SubjectCode", programSubject.SubjectCode},
                { "@CourseId", programSubject.CourseId},
                { "@Name", programSubject.Name},
                { "@Credit", programSubject.Credit},
                { "@CurriculumId", programSubject.CurriculumId},
            };

            _rawSql.ExecNonQuery(sb.ToString(), param);
        }

        public bool ExistsByCourseId(string courseId)
        {
            return _rawSql.ExecScalar(
                "SELECT COUNT(*) FROM DbProgramSubjects WHERE CourseId = @courseId"
                , new Dictionary<string, object>()
                {
                    {"@courseId", courseId }
                }
                , 0L
                ) >= 1L;
        }

        public List<DtoDbProgramSubject> GetDbProgramSubjectsByCrrId(int crrId)
        {
            var sql = new StringBuilder();
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
            var param = new Dictionary<string, object>
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
            );
        }
    }
}
