using Cs4rsa.Cs4rsaDatabase.DataProviders;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;

using System;
using System.Collections.Generic;
using System.Text;

namespace Cs4rsa.Cs4rsaDatabase.Implements
{
    public class CurriculumRepository : ICurriculumRepository
    {
        public CurriculumRepository()
        {
        }

        public List<Curriculum> GetAllCurr()
        {
            return RawSql.ExecReader(
                "SELECT * FROM Curriculums"
                , record => new Curriculum()
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
            Dictionary<string, object> param = new()
            {
                {"@CurriculumId", currId}
            };
            long result = RawSql.ExecScalar(
                "SELECT COUNT(*) FROM DbProgramSubjects WHERE CurriculumId = @CurriculumId"
                , param
                , 0L);
            return Convert.ToInt32(result);
        }

        public bool ExistsById(int currId)
        {
            Dictionary<string, object> param = new()
            {
                {"@CurriculumId", currId}
            };
            return RawSql.ExecScalar(
                "SELECT COUNT(*) FROM Curriculums WHERE CurriculumId = @CurriculumId LIMIT 1"
                , param
                , 0L) == 1L;
        }

        public Curriculum GetByID(int currId)
        {
            StringBuilder sb = new StringBuilder()
                .AppendLine("SELECT")
                .AppendSelectColumns<Curriculum>()
                .AppendLine("FROM Curriculums")
                .AppendLine("WHERE CurriculumId = @CurriculumId")
                .AppendLine("LIMIT 1");
            Dictionary<string, object> param = new()
            {
                {"@CurriculumId", currId}
            };
            return RawSql.ExecReaderGetFirstOrDefault(
                sb.ToString(),
                new Dictionary<string, object>()
                {
                    {"@CurriculumId", currId}
                },
                r => new Curriculum()
                {
                    CurriculumId = r.GetInt32(0),
                    Name = r.GetString(1),
                }
            );
        }

        public int Insert(Curriculum curriculum)
        {
            StringBuilder sb = new StringBuilder()
                .AppendLine("INSERT INTO Curriculums")
                .AppendLine("VALUES")
                .AppendLine("(@id, @name)");
            Dictionary<string, object> param = new()
            {
                {"@id", curriculum.CurriculumId },
                {"@name", curriculum.Name },
            };
            return RawSql.ExecNonQuery(sb.ToString(), param);
        }
    }
}
