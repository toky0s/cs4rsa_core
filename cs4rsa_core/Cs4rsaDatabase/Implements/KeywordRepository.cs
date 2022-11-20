using Cs4rsa.Cs4rsaDatabase.DataProviders;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;

using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cs4rsa.Cs4rsaDatabase.Implements
{
    public class KeywordRepository : GenericRepository<Keyword>, IKeywordRepository
    {
        public KeywordRepository(Cs4rsaDbContext context) : base(context)
        {
        }

        public async Task<string> GetColorAsync(int courseId)
        {
            Keyword keyword = await _context.Keywords.FirstOrDefaultAsync(keyword => keyword.CourseId == courseId);
            return keyword != null ? keyword.Color : "";
        }

        public string GetColorWithSubjectCode(string subjectCode)
        {
            Keyword keyword = (from discipline in _context.Disciplines
                               join kw in _context.Keywords on discipline.DisciplineId equals kw.DisciplineId
                               where discipline.Name + " " + kw.Keyword1 == subjectCode
                               select kw).FirstOrDefault();
            return keyword.Color;
        }

        public int GetCourseId(string subjectCode)
        {
            char[] splitChar = { ' ' };
            string[] slices = subjectCode.Split(splitChar);
            return _context.Keywords
                .Where(kw => kw.Discipline.Name == slices[0] && kw.Keyword1 == slices[1])
                .FirstOrDefault()
                .CourseId;
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

        public bool IsHasColor(string color)
        {
            return _context.Keywords.Where(kw => kw.Color == color).Any();
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
            char[] splitChars = { ' ' };
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

        public Task<List<Keyword>> GetBySubjectNameContains(string subjectName)
        {
            subjectName = subjectName.Trim();
            return _context.Keywords
                .Where(kw => kw.SubjectName.ToLower().Contains(subjectName))
                .Take(20)
                .ToListAsync();
        }

        public Task<List<Keyword>> GetByDisciplineAndKeyword1(string discipline, string keyword)
        {
            return _context.Keywords.Where(kw =>
                kw.Discipline.Name.Contains(discipline.ToUpper())
                && kw.Keyword1.Contains(keyword)
            ).Take(20).ToListAsync();
        }

        public Task<List<Keyword>> GetByDisciplineStartWith(string text)
        {
            return _context.Keywords
                .Where(kw => kw.Discipline.Name.ToUpper().StartsWith(text.ToUpper()))
                .Take(20)
                .ToListAsync();
        }

        public async Task<string> GetSubjectCode(int courseId)
        {
            var result = await (from dis in _context.Disciplines
                                join kw in _context.Keywords
                                on new { jprop1 = dis.DisciplineId, jprop2 = courseId, }
                                equals new { jprop1 = kw.DisciplineId, jprop2 = kw.CourseId, }
                                select new { dis.Name, kw.Keyword1 }).FirstOrDefaultAsync();
            return result.Name + " " + result.Keyword1;
        }
    }
}
