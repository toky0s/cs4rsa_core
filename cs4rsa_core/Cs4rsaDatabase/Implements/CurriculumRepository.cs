using Cs4rsa.Cs4rsaDatabase.DataProviders;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;

using System;
using System.Collections.Generic;

namespace Cs4rsa.Cs4rsaDatabase.Implements
{
    public class CurriculumRepository : GenericRepository<Curriculum>, ICurriculumRepository
    {
        public CurriculumRepository(Cs4rsaDbContext context) : base(context)
        {
        }

        IEnumerable<Curriculum> ICurriculumRepository.GetAllCurr()
        {
            return _rawSql.ExecReader(
                "SELECT * FROM Curriculums"
                , null
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
            long result = _rawSql.ExecScalar(
                "SELECT COUNT(*) FROM DbProgramSubjects WHERE CurriculumId = @CurriculumId"
                , param
                , 0L
                , true);
            return Convert.ToInt32(result);
        }

        public bool ExistsById(string currId)
        {
            Dictionary<string, object> param = new()
            {
                {"@CurriculumId", currId}
            };
            return _rawSql.ExecScalar(
                "SELECT COUNT(*) FROM Curriculums WHERE CurriculumId = @CurriculumId LIMIT 1"
                , param
                , 0L) == 1L;
        }
    }
}
