using Cs4rsa.Constants;
using Cs4rsa.Cs4rsaDatabase.DataProviders;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;

using System.Collections.Generic;
using System.Text;

namespace Cs4rsa.Cs4rsaDatabase.Implements
{
    public class KeywordRepository : IKeywordRepository
    {
        public string GetColor(int courseId)
        {
            return RawSql.ExecScalar(
                "SELECT Color FROM Keywords WHERE CourseId = @CourseID"
                , new Dictionary<string, object>()
                {
                    { "@CourseID", courseId }
                }
                , string.Empty
            );
        }

        public string GetColorWithSubjectCode(string subjectCode)
        {
            StringBuilder sb = new();
            sb.AppendLine("SELECT Color");
            sb.AppendLine("FROM Disciplines AS ds");
            sb.AppendLine("   , Keywords AS kw");
            sb.AppendLine("WHERE ds.Name || ' ' || kw.Keyword1 = @subjectCode");
            sb.AppendLine("AND ds.DisciplineId = kw.DisciplineId");
            return RawSql.ExecScalar(
                sb.ToString()
                , new Dictionary<string, object>()
                {
                    { "@subjectCode", subjectCode }
                }
                , string.Empty
            );
        }

        public Keyword GetKeyword(string discipline, string keyword1)
        {
            StringBuilder sb = new();
            sb.AppendLine("SELECT");
            sb.AppendLine("  kw.KeywordId");
            sb.AppendLine(", kw.Keyword1");
            sb.AppendLine(", kw.CourseId");
            sb.AppendLine(", kw.SubjectName");
            sb.AppendLine(", kw.Color");
            sb.AppendLine(", kw.Cache");
            sb.AppendLine(", kw.DisciplineId");
            sb.AppendLine("FROM Disciplines AS ds");
            sb.AppendLine("	  , Keywords    AS kw");
            sb.AppendLine("WHERE ds.DisciplineId = kw.DisciplineId");
            sb.AppendLine("	AND ds.Name = @discipline");
            sb.AppendLine("	AND kw.Keyword1 = @keyword1");
            return RawSql.ExecReaderGetFirstOrDefault(
                sb.ToString(),
                new Dictionary<string, object>()
                {
                    { "@discipline",  discipline },
                    { "@keyword1", keyword1 } 
                },
                record =>
                {
                    return new Keyword()
                    {
                        KeywordId = record.GetInt32(0),
                        Keyword1 = record.GetString(1),
                        CourseId = record.GetInt32(2),
                        SubjectName = record.GetString(3),
                        Color = record.GetString(4),
                        Cache = record.IsDBNull(5) ? null : record.GetString(5),
                        DisciplineId = record.GetInt32(6)
                    };
                }
            );
        }

        public Keyword GetKeyword(int courseId)
        {
            StringBuilder sb = new();
            sb.AppendLine("SELECT");
            sb.AppendLine("  KeywordId");
            sb.AppendLine(", Keyword1");
            sb.AppendLine(", CourseId");
            sb.AppendLine(", SubjectName");
            sb.AppendLine(", Color");
            sb.AppendLine(", Cache");
            sb.AppendLine(", DisciplineId");
            sb.AppendLine("FROM Keywords");
            sb.AppendLine("WHERE CourseId = @courseId");
            Dictionary<string, object> param = new()
            {
                {"@courseId", courseId }
            };
            return RawSql.ExecReaderGetFirstOrDefault(
                sb.ToString()
                , param
                , r => new Keyword()
                {
                      KeywordId = r.GetInt32(0)
                    , Keyword1 = r.GetString(1)
                    , CourseId = r.GetInt32(2)
                    , SubjectName = r.GetString(3)
                    , Color = r.GetString(4)
                    , Cache = r.IsDBNull(5) ? null : r.GetString(5)
                    , DisciplineId = r.GetInt32(6)
                }
            );
        }

        public Keyword GetKeyword(string subjectCode)
        {
            char[] splitChars = { VmConstants.CharSpace };
            string[] slices = subjectCode.Split(splitChars);
            return GetKeyword(slices[0], slices[1]);
        }

        public long Count(string discipline, string keyword1)
        {
            StringBuilder sb = new();
            sb.AppendLine("SELECT COUNT(*)");
            sb.AppendLine("FROM Disciplines AS ds");
            sb.AppendLine("	  , Keywords    AS kw");
            sb.AppendLine("WHERE ds.DisciplineId = kw.DisciplineId");
            sb.AppendLine("	AND ds.Name = @discipline");
            sb.AppendLine("	AND kw.Keyword1 = @keyword1");
            return RawSql.ExecScalar(
                sb.ToString()
                , new Dictionary<string, object>()
                {
                    {"@discipline", discipline },
                    {"@keyword1", keyword1 }
                }
                , 0L
            );
        }

        public string GetCache(string courseId)
        {
            string sql = "SELECT Cache FROM Keywords WHERE CourseId = @CourseId";
            Dictionary<string, object> param = new()
            {
                { "@CourseId", courseId}
            };
            return RawSql.ExecScalar(sql, param, string.Empty);
        }

        public string GetColorBySubjectCode(string subjectCode)
        {
            StringBuilder sb = new();
            sb.AppendLine("SELECT Color");
            sb.AppendLine("FROM Keywords AS kw");
            sb.AppendLine("INNER JOIN Disciplines AS ds");
            sb.AppendLine("ON ds.DisciplineId = kw.DisciplineId");
            sb.AppendLine("WHERE ds.Name || ' ' || kw.Keyword1 = @SubjectCode");
            Dictionary<string, object> param = new()
            {
                { "@SubjectCode", subjectCode}
            };
            return RawSql.ExecScalar(sb.ToString(), param, string.Empty);
        }

        public List<Keyword> GetKeywordsByDisciplineId(int disciplineId)
        {
            StringBuilder sb = new();
            sb.AppendLine("SELECT KeywordId, Keyword1, CourseId, SubjectName, Color, Cache");
            sb.AppendLine("FROM Keywords");
            sb.AppendLine("WHERE DisciplineId = @DisciplineId");
            Dictionary<string, object> param = new()
            {
                { "@DisciplineId", disciplineId}
            };
            return RawSql.ExecReader(sb.ToString(), param, record =>
                new Keyword()
                {
                      KeywordId = record.GetInt32(0)
                    , Keyword1 = record.GetString(1)
                    , CourseId = record.GetInt32(2)
                    , SubjectName = record.GetString(3)
                    , Color = record.GetString(4)
                    , Cache = record.IsDBNull(5) ? string.Empty : record.GetString(5)
                }
            );
        }

        public int UpdateCacheByKeywordID(int keywordID, string cache)
        {
            return RawSql.ExecNonQuery(
                "UPDATE Keywords SET Cache = @cache WHERE KeywordId = @keywordID"
                , new Dictionary<string, object>()
                {
                    {"@cache", cache },
                    {"@keywordID", keywordID },
                });
        }

        public int Insert(Keyword keyword)
        {
            StringBuilder sb = new StringBuilder()
                .AppendLine("INSERT INTO Keywords")
                .AppendLine("VALUES")
                .AppendLine("(@KeywordId, @Keyword1, @CourseId, @SubjectName, @Color, @Cache, @DisciplineId)");
            return RawSql.ExecNonQuery(
                sb.ToString()
                , new Dictionary<string, object>()
                {
                    { "@KeywordId", keyword.KeywordId},
                    { "@Keyword1", keyword.Keyword1},
                    { "@CourseId", keyword.CourseId},
                    { "@SubjectName", keyword.SubjectName},
                    { "@Color", keyword.Color},
                    { "@Cache", keyword.Cache},
                    { "@DisciplineId", keyword.DisciplineId},
                }
            );
        }

        public int DeleteAll()
        {
            return RawSql.ExecNonQuery("DELETE FROM Keywords");
        }

        public Keyword GetByCourseId(int intCourseId)
        {
            StringBuilder sb = new StringBuilder()
                .AppendLine("SELECT kw.KeywordId")
                .AppendLine(", kw.Keyword1")
                .AppendLine(", kw.CourseId")
                .AppendLine(", kw.SubjectName")
                .AppendLine(", kw.Color")
                .AppendLine(", kw.Cache")
                .AppendLine(", ds.DisciplineId")
                .AppendLine(", ds.Name")
                .AppendLine("FROM")
                .AppendLine("  Keywords AS kw")
                .AppendLine(", Disciplines AS ds")
                .AppendLine("WHERE")
                .AppendLine("    CourseId = @CourseId")
                .AppendLine("AND kw.DisciplineId = ds.DisciplineId");
            Dictionary<string, object> param = new()
            {
                { "@CourseId", intCourseId}
            };
            return RawSql.ExecReaderGetFirstOrDefault(
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
                    , DisciplineId = record.GetInt32(6)
                    , Discipline = 
                    new Discipline() 
                    { 
                          DisciplineId = record.GetInt32(6)
                        , Name = record.GetString(7)
                    }
                }
            );
        }

        public long Count()
        {
            return RawSql.ExecScalar("SELECT COUNT(*) FROM Keywords", 0L);
        }
    }
}