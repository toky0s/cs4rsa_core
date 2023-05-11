using Cs4rsa.Cs4rsaDatabase.DataProviders;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cs4rsa.Cs4rsaDatabase.Implements
{
    public class DisciplineRepository : GenericRepository<Discipline>, IDisciplineRepository
    {
        public DisciplineRepository(Cs4rsaDbContext context) : base(context)
        {

        }

        public IEnumerable<Discipline> GetAllDiscipline()
        {
            string sql = "SELECT DisciplineId, Name FROM Disciplines";
            return _rawSql.ExecReader(sql, null, record =>
                new Discipline()
                {
                    DisciplineId = record.GetInt32(0),
                    Name = record.GetString(1)
                }
            );
        }

        public IEnumerable<Discipline> GetAllIncludeKeyword()
        {
            string sql = "SELECT DisciplineId, Name FROM Disciplines";
            IEnumerable<Discipline> disciplines = _rawSql.ExecReader(
                sql
                , null
                , record =>
                new Discipline()
                {
                    DisciplineId = record.GetInt32(0),
                    Name = record.GetString(1)
                }
            );

            StringBuilder sb = new();
            sb.AppendLine("SELECT KeywordId, Keyword1, CourseId, SubjectName, Color, Cache");
            sb.AppendLine("FROM Keywords");
            sb.AppendLine("WHERE DisciplineId = @DisciplineId");

            foreach (Discipline discipline in disciplines)
            {
                Dictionary<string, object> param = new()
                {
                    { "@DisciplineId", discipline.DisciplineId}
                };
                discipline.Keywords = _rawSql.ExecReader(
                    sb.ToString()
                    , param
                    , record =>
                        new Keyword()
                        {
                              KeywordId = record.GetInt32(0)
                            , Keyword1 = record.GetString(1)
                            , CourseId = record.GetInt32(2)
                            , SubjectName = record.GetString(3)
                            , Color = record.GetString(4)
                            , Cache = record.IsDBNull(5) ? string.Empty : record.GetString(5)
                            , Discipline = discipline
                            , DisciplineId = discipline.DisciplineId
                        }
                    , isNestedOrInIEnumerable: true
                ).ToList();
                yield return discipline;
            }
        }

        public Discipline GetDisciplineByID(int id)
        {
            StringBuilder sb = new();
            sb.AppendLine("SELECT DisciplineId, Name ");
            sb.AppendLine("FROM Disciplines ");
            sb.AppendLine("WHERE DisciplineId = @id");
            sb.AppendLine("LIMIT 1");
            return _rawSql.ExecReaderGetFirstOrDefault(
                sb.ToString()
                , new Dictionary<string, object>()
                {
                    { "@id", id }
                }
                , r => new Discipline() { DisciplineId = id, Name = r.GetString(1) }
            );
        }
    }
}
