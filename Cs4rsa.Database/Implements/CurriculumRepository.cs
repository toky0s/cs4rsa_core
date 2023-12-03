using System;
using System.Collections.Generic;
using System.Text;
using Cs4rsa.Database.DataProviders;
using Cs4rsa.Database.Interfaces;
using Cs4rsa.Database.Models;

namespace Cs4rsa.Database.Implements
{
    public class CurriculumRepository : ICurriculumRepository
    {
        private readonly RawSql _rawSql;

        public CurriculumRepository(RawSql rawSql)
        {
            _rawSql = rawSql;
        }
        
        public List<Curriculum> GetAllCurr()
        {
            return _rawSql.ExecReader(
                "SELECT * FROM Curriculums"
                , record => new Curriculum
                {
                    CurriculumId = record.GetInt32(0),
                    Name = record.IsDBNull(1)
                            ? "Chưa thể xác định"
                            : record.GetString(1)
                }
            );
        }

        public int GetCountMajorSubjectByCurrId(int currId)
        {
            var param = new Dictionary<string, object>()
            {
                {"@CurriculumId", currId}
            };
            var result = _rawSql.ExecScalar(
                "SELECT COUNT(*) FROM DbProgramSubjects WHERE CurriculumId = @CurriculumId"
                , param
                , 0L);
            return Convert.ToInt32(result);
        }

        public bool ExistsById(int currId)
        {
            var param = new Dictionary<string, object>
            {
                {"@CurriculumId", currId}
            };
            return _rawSql.ExecScalar(
                "SELECT COUNT(*) FROM Curriculums WHERE CurriculumId = @CurriculumId LIMIT 1"
                , param
                , 0L) == 1L;
        }

        public Curriculum GetByID(int currId)
        {
            var sb = new StringBuilder()
                .AppendLine("SELECT")
                .AppendSelectColumns<Curriculum>(_rawSql.CnnStr)
                .AppendLine("FROM Curriculums")
                .AppendLine("WHERE CurriculumId = @CurriculumId")
                .AppendLine("LIMIT 1");
            var param = new Dictionary<string, object>
            {
                {"@CurriculumId", currId}
            };
            return _rawSql.ExecReaderGetFirstOrDefault(
                sb.ToString(),
                param,
                r => new Curriculum()
                {
                    CurriculumId = r.GetInt32(0),
                    Name = r.GetString(1),
                }
            );
        }

        public int Insert(Curriculum curriculum)
        {
            var sb = new StringBuilder()
                .AppendLine("INSERT INTO Curriculums")
                .AppendLine("VALUES")
                .AppendLine("(@id, @name)");
            var param = new Dictionary<string, object>()
            {
                {"@id", curriculum.CurriculumId },
                {"@name", curriculum.Name },
            };
            return _rawSql.ExecNonQuery(sb.ToString(), param);
        }
    }
}
