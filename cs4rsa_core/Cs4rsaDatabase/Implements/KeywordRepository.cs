using Cs4rsa.Constants;
using Cs4rsa.Cs4rsaDatabase.DataProviders;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;

using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsa.Cs4rsaDatabase.Implements
{
    public class KeywordRepository : GenericRepository<Keyword>, IKeywordRepository
    {
        public KeywordRepository(Cs4rsaDbContext context) : base(context)
        {
        }

        public string GetColor(int courseId)
        {
            return _rawSql.ExecScalar(
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
            return _rawSql.ExecScalar(
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
            return _rawSql.ExecReaderGetFirstOrDefault(
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

        public async Task<Keyword> GetKeyword(int courseId)
        {
            IQueryable<Keyword> keywordByDisciplineAndKeyword1Query = from kw in _context.Keywords
                                                                      where kw.CourseId == courseId
                                                                      select kw;
            return await keywordByDisciplineAndKeyword1Query.FirstOrDefaultAsync();
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
            return _rawSql.ExecScalar(
                sb.ToString()
                , new Dictionary<string, object>()
                {
                    {"@discipline", discipline },
                    {"@keyword1", keyword1 }
                }
                , 0L
            );
        }

        public async Task<bool> ExistBySubjectCodeAsync(string subjectCode)
        {
            char[] splitChars = { VmConstants.CharSpace };
            string[] slices = subjectCode.Split(splitChars);
            return await (
                from ds in _context.Disciplines
                from kw in _context.Keywords
                where ds.Name == slices[0] && kw.Keyword1 == slices[1]
                && ds.DisciplineId == kw.DisciplineId
                select kw
            ).AnyAsync();
        }

        public List<Keyword> GetSearchResult(string searchText, int limit)
        {
            StringBuilder sqlBuilder = new();
            sqlBuilder.AppendLine("SELECT *");
            sqlBuilder.AppendLine("FROM Keywords AS k");
            sqlBuilder.AppendLine("    INNER JOIN Disciplines AS d");
            sqlBuilder.AppendLine("    ON k.SubjectName LIKE @pattern");
            sqlBuilder.AppendLine("    AND d.DisciplineId = k.DisciplineId");
            sqlBuilder.AppendLine("UNION");
            sqlBuilder.AppendLine("SELECT *");
            sqlBuilder.AppendLine("FROM Keywords AS k");
            sqlBuilder.AppendLine("    INNER JOIN Disciplines AS d");
            sqlBuilder.AppendLine("    ON d.Name || ' ' || k.Keyword1 LIKE @pattern");
            sqlBuilder.AppendLine("    AND d.DisciplineId = k.DisciplineId");
            sqlBuilder.AppendLine("UNION");
            sqlBuilder.AppendLine("SELECT *");
            sqlBuilder.AppendLine("FROM Keywords AS k");
            sqlBuilder.AppendLine("    INNER JOIN Disciplines AS d");
            sqlBuilder.AppendLine($"   ON {RawSql.UseFunction<FuncRemoveAccent>("k.SubjectName")} LIKE @pattern");
            sqlBuilder.AppendLine("    AND d.DisciplineId = k.DisciplineId");
            sqlBuilder.AppendLine("LIMIT (@limit);");

            IDictionary<string, object> sqlParams = new Dictionary<string, object>
            {
                { "@pattern", $"%{searchText}%" },
                { "@limit", limit }
            };

            return _rawSql.ExecReader(sqlBuilder.ToString(), sqlParams, (record) =>
            {
                Discipline ds = new()
                {
                    DisciplineId = record.GetInt32(7),
                    Name = record.GetString(8)
                };
                Keyword kw = new()
                {
                    KeywordId = record.GetInt32(0),
                    Keyword1 = record.GetString(1),
                    CourseId = record.GetInt32(2),
                    SubjectName = record.GetString(3),
                    Color = record.GetString(4),
                    Cache = record.IsDBNull(5) ? null : record.GetString(5),
                    DisciplineId = record.GetInt32(6),
                    Discipline = ds
                };
                return kw;
            }).ToList();
        }

        public string GetCache(string courseId)
        {
            string sql = "SELECT Cache FROM Keywords WHERE CourseId = @CourseId";
            Dictionary<string, object> param = new()
            {
                { "@CourseId", courseId}
            };
            return _rawSql.ExecScalar(sql, param, string.Empty);
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
            return _rawSql.ExecScalar(sb.ToString(), param, string.Empty);
        }

        public IEnumerable<Keyword> GetKeywordsByDisciplineId(int disciplineId)
        {
            StringBuilder sb = new();
            sb.AppendLine("SELECT KeywordId, Keyword1, CourseId, SubjectName, Color, Cache");
            sb.AppendLine("FROM Keywords");
            sb.AppendLine("WHERE DisciplineId = @DisciplineId");
            Dictionary<string, object> param = new()
            {
                { "@DisciplineId", disciplineId}
            };
            return _rawSql.ExecReader(sb.ToString(), param, record =>
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
    }
}
