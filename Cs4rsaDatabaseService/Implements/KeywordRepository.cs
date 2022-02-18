using Cs4rsaDatabaseService.DataProviders;
using Cs4rsaDatabaseService.Interfaces;
using Cs4rsaDatabaseService.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cs4rsaDatabaseService.Implements
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

        public Keyword GetKeyword(string discipline, string keyword1)
        {
            IQueryable<Keyword> keywordByDisciplineAndKeyword1Query = from ds in _context.Disciplines
                                                                      from kw in _context.Keywords
                                                                      where ds.Name == discipline && kw.Keyword1 == keyword1
                                                                      && ds.DisciplineId == kw.DisciplineId
                                                                      select kw;
            return keywordByDisciplineAndKeyword1Query.FirstOrDefault();
        }

        public bool IsHasColor(string color)
        {
            return _context.Keywords.Where(kw => kw.Color == color).Any();
        }

        public Keyword GetKeyword(int courseId)
        {
            IQueryable<Keyword> keywordByDisciplineAndKeyword1Query = from kw in _context.Keywords
                                                                      where kw.CourseId == courseId
                                                                      select kw;
            return keywordByDisciplineAndKeyword1Query.FirstOrDefault();
        }

        public Keyword GetKeyword(string subjectCode)
        {
            char[] splitChars = { ' ' };
            string[] slices = subjectCode.Split(splitChars);
            return GetKeyword(slices[0], slices[1]);
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
            return _context.Keywords.Where(kw => kw.SubjectName.Contains(subjectName))
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
            return _context.Keywords.Where(kw =>
                kw.Discipline.Name.StartsWith(text.ToUpper())
            ).Take(20).ToListAsync();
        }
    }
}
