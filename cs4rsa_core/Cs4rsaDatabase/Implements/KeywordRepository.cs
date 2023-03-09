using Cs4rsa.Constants;
using Cs4rsa.Cs4rsaDatabase.DataProviders;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;

using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Cs4rsa.Cs4rsaDatabase.Implements
{
    public class KeywordRepository : GenericRepository<Keyword>, IKeywordRepository
    {
        public KeywordRepository(Cs4rsaDbContext context, RawSql rawSql) : base(context, rawSql)
        {
        }

        public async Task<string> GetColorAsync(int courseId)
        {
            Keyword keyword = await _context.Keywords.FirstOrDefaultAsync(keyword => keyword.CourseId == courseId);
            return keyword != null ? keyword.Color : string.Empty;
        }

        public string GetColorWithSubjectCode(string subjectCode)
        {
            Keyword keyword = (from discipline in _context.Disciplines
                               join kw in _context.Keywords on discipline.DisciplineId equals kw.DisciplineId
                               where discipline.Name + VmConstants.StrSpace + kw.Keyword1 == subjectCode
                               select kw).FirstOrDefault();
            return keyword.Color;
        }

        public async Task<Keyword> GetKeyword(string discipline, string keyword1)
        {
            IQueryable<Keyword> keywordByDisciplineAndKeyword1Query = from ds in _context.Disciplines
                                                                      from kw in _context.Keywords
                                                                      where ds.Name == discipline && kw.Keyword1 == keyword1
                                                                      && ds.DisciplineId == kw.DisciplineId
                                                                      select kw;
            return await keywordByDisciplineAndKeyword1Query.FirstOrDefaultAsync();
        }

        public async Task<Keyword> GetKeyword(int courseId)
        {
            IQueryable<Keyword> keywordByDisciplineAndKeyword1Query = from kw in _context.Keywords
                                                                      where kw.CourseId == courseId
                                                                      select kw;
            return await keywordByDisciplineAndKeyword1Query.FirstOrDefaultAsync();
        }

        public async Task<Keyword> GetKeyword(string subjectCode)
        {
            char[] splitChars = { VmConstants.CharSpace };
            string[] slices = subjectCode.Split(splitChars);
            return await GetKeyword(slices[0], slices[1]);
        }

        public async Task<int> CountAsync(string discipline, string keyword)
        {
            var query = from d in _context.Disciplines
                        join k in _context.Keywords
                        on d.DisciplineId equals k.DisciplineId
                        where d.Name == discipline && k.Keyword1 == keyword
                        select k;
            return await query.CountAsync();
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
            string sql =
                     $" SELECT *"
            + "\n" + $" FROM Keywords AS k"
            + "\n" + $"     INNER JOIN Disciplines AS d"
            + "\n" + $"     ON k.SubjectName LIKE \"%{searchText}%\""
            + "\n" + $"     AND d.DisciplineId = k.DisciplineId"
            + "\n" + $" UNION"
            + "\n" + $" SELECT *"
            + "\n" + $" FROM Keywords AS k"
            + "\n" + $"     INNER JOIN Disciplines AS d"
            + "\n" + $"     ON d.Name || \" \" || k.Keyword1 LIKE \"%{searchText}%\""
            + "\n" + $"     AND d.DisciplineId = k.DisciplineId"
            + "\n" + $" LIMIT {limit};";

            return _rawSql.ExecReader(sql, (reader) =>
            {
                List<Keyword> result = new();
                while (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Discipline ds = new()
                        {
                            DisciplineId = reader.GetInt32(7),
                            Name = reader.GetString(8)
                        };
                        Keyword kw = new()
                        {
                            KeywordId = reader.GetInt32(0),
                            Keyword1 = reader.GetString(1),
                            CourseId = reader.GetInt32(2),
                            SubjectName = reader.GetString(3),
                            Color = reader.GetString(4),
                            Cache = reader.IsDBNull(5) ? null : reader.GetString(5),
                            DisciplineId = reader.GetInt32(6),
                            Discipline = ds
                        };
                        result.Add(kw);
                    }
                    reader.NextResult();
                }
                return result;
            });
        }
    }
}
