using Cs4rsa.Cs4rsaDatabase.DataProviders;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;

using System.Collections.Generic;
using System.Text;

namespace Cs4rsa.Cs4rsaDatabase.Implements
{
    public class DisciplineRepository : IDisciplineRepository
    {
        public int DeleteAll()
        {
            return RawSql.ExecNonQuery("DELETE FROM Disciplines");
        }

        public List<Discipline> GetAllDiscipline()
        {
            string sql = "SELECT DisciplineId, Name FROM Disciplines";
            return RawSql.ExecReader(sql, null, record =>
                new Discipline()
                {
                    DisciplineId = record.GetInt32(0),
                    Name = record.GetString(1)
                }
            );
        }

        public List<Discipline> GetAllIncludeKeyword()
        {
            string sql = "SELECT DisciplineId, Name FROM Disciplines";
            List<Discipline> disciplines = RawSql.ExecReader(
                sql
                , null
                , record => new Discipline()
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
                discipline.Keywords = RawSql.ExecReader(
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
                );
            }
            return disciplines;
        }

        public Discipline GetDisciplineByID(int id)
        {
            StringBuilder sb = new();
            sb.AppendLine("SELECT DisciplineId, Name ");
            sb.AppendLine("FROM Disciplines ");
            sb.AppendLine("WHERE DisciplineId = @id");
            sb.AppendLine("LIMIT 1");
            return RawSql.ExecReaderGetFirstOrDefault(
                sb.ToString()
                , new Dictionary<string, object>()
                {
                    { "@id", id }
                }
                , r => new Discipline() { DisciplineId = id, Name = r.GetString(1) }
            );
        }

        public int Insert(Discipline discipline)
        {
            StringBuilder sb = new StringBuilder()
                .AppendLine("INSERT INTO Disciplines")
                .AppendLine("VALUES")
                .AppendLine("(@DisciplineId, @Name)");
            Dictionary<string, object> param = new()
            {
                  {"@DisciplineId", discipline.DisciplineId }
                , {"@Name", discipline.Name }
            };
            return RawSql.ExecNonQuery(sb.ToString(), param);
        }
    }
}
