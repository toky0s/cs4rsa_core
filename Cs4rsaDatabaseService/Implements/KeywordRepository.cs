using Cs4rsaDatabaseService.DataProviders;
using Cs4rsaDatabaseService.Interfaces;
using Cs4rsaDatabaseService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsaDatabaseService.Implements
{
    public class KeywordRepository : GenericRepository<Keyword>, IKeywordRepository
    {
        public KeywordRepository(Cs4rsaDbContext context) : base(context)
        {
        }

        public string GetColor(int courseId)
        {
            Keyword keyword = _context.Keywords.Where(keyword => keyword.CourseId == courseId).FirstOrDefault();
            return keyword.Color;
        }

        public string GetColorWithSubjectCode(string subjectCode)
        {
            Keyword keyword = (from discipline in _context.Disciplines
                               from kw in _context.Keywords
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
    }
}
