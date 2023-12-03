using System.Collections.Generic;
using System.Text;
using Cs4rsa.Database.DataProviders;
using Cs4rsa.Database.Interfaces;
using Cs4rsa.Database.Models;

namespace Cs4rsa.Database.Implements
{
    public class DisciplineRepository : IDisciplineRepository
    {
        private readonly RawSql _rawSql;

        public DisciplineRepository(RawSql rawSql)
        {
            _rawSql = rawSql;
        }
        public int DeleteAll()
        {
            return _rawSql.ExecNonQuery("DELETE FROM Disciplines");
        }

        public List<Discipline> GetAllDiscipline()
        {
            const string sql = "SELECT DisciplineId, Name FROM Disciplines";
            return _rawSql.ExecReader(sql, null, record =>
                new Discipline
                {
                    DisciplineId = record.GetInt32(0),
                    Name = record.GetString(1)
                }
            );
        }

        public List<Discipline> GetAllIncludeKeyword()
        {
            const string sql = "SELECT DisciplineId, Name FROM Disciplines";
            var disciplines = _rawSql.ExecReader(
                sql
                , null
                , record => new Discipline()
                {
                    DisciplineId = record.GetInt32(0),
                    Name = record.GetString(1)
                }
            );

            var sb = new StringBuilder();
            sb.AppendLine("SELECT KeywordId, Keyword1, CourseId, SubjectName, Color, Cache");
            sb.AppendLine("FROM Keywords");
            sb.AppendLine("WHERE DisciplineId = @DisciplineId");

            foreach (var discipline in disciplines)
            {
                var param = new Dictionary<string, object>()
                {
                    { "@DisciplineId", discipline.DisciplineId}
                };
                discipline.Keywords = _rawSql.ExecReader(
                    sb.ToString()
                    , param
                    , record =>
                    new Keyword
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
                );
            }
            return disciplines;
        }

        public Discipline GetDisciplineByID(int id)
        {
            var sb = new StringBuilder();
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
                , r => new Discipline { DisciplineId = id, Name = r.GetString(1) }
            );
        }

        public int Insert(Discipline discipline)
        {
            var sb = new StringBuilder()
                .AppendLine("INSERT INTO Disciplines")
                .AppendLine("VALUES")
                .AppendLine("(@DisciplineId, @Name)");
            var param = new Dictionary<string, object>()
            {
                  {"@DisciplineId", discipline.DisciplineId }
                , {"@Name", discipline.Name }
            };
            return _rawSql.ExecNonQuery(sb.ToString(), param);
        }
    }
}
