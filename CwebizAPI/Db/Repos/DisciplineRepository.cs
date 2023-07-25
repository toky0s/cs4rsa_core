/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using System.Diagnostics;
using CwebizAPI.Db.Interfaces;
using Microsoft.EntityFrameworkCore;
using CwebizAPI.Share.Database.Models;
using CwebizAPI.Utils;

namespace CwebizAPI.Db.Repos
{
    /// <summary>
    /// Discipline Repository.
    /// </summary>
    /// <remarks>
    /// Created Date: 21/06/2023
    /// Modified Date: 21/06/2023
    /// Author: Truong A Xin
    /// </remarks>
    public sealed class DisciplineRepository : IDisciplineRepository
    {
        private bool _disposed;
        private readonly CwebizContext _dbContext;

        public DisciplineRepository(CwebizContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void DeleteAllDisciplineAndKeyword()
        {
            _dbContext.Keywords!.RemoveRange(_dbContext.Keywords);
            _dbContext.Disciplines!.RemoveRange(_dbContext.Disciplines);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<List<Discipline>> GetAllDiscipline()
        {
            return await (from ds in _dbContext.Disciplines
                orderby ds.Name
                select ds).ToListAsync();
        }

        public IEnumerable<Discipline?> GetAllIncludeKeyword()
        {
            return _dbContext.Disciplines!.Include(nameof(Keyword));
        }

        public async Task<Discipline?> GetDisciplineById(int id)
        {
            return await _dbContext.Disciplines!.FirstOrDefaultAsync(discipline => discipline.Id.Equals(id));
        }

        public void Insert(Discipline? discipline)
        {
            _dbContext.Disciplines!.Add(discipline!);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }

            _disposed = true;
        }

        public async Task<Keyword?> GetKeyword(string discipline, string keyword1)
        {
            return (await (from kw in _dbContext.Keywords
                from ds in _dbContext.Disciplines
                where kw.Keyword1 == keyword1
                      && ds.Name!.Equals(discipline)
                      && ds.Id == kw.DisciplineId
                select kw).FirstOrDefaultAsync());
        }

        public async Task<Keyword?> GetKeywordByCourseId(string courseId)
        {
            return (await (from kw in _dbContext.Keywords
                where kw.CourseId.Equals(courseId)
                select kw).FirstOrDefaultAsync());
        }

        public async Task<Keyword> GetKeyword(string subjectCode)
        {
            return (await (from kw in _dbContext.Keywords
                from ds in _dbContext.Disciplines
                where (ds.Name + " " + kw.Id).Equals(subjectCode)
                      && ds.Id == kw.DisciplineId
                select kw).FirstOrDefaultAsync())!;
        }

        public async Task<string> GetColorWithSubjectCode(string subjectCode)
        {
            return (await (from kw in _dbContext.Keywords
                from ds in _dbContext.Disciplines
                where (ds.Name + " " + kw.Id).Equals(subjectCode)
                      && ds.Id == kw.DisciplineId
                select kw.Color).FirstOrDefaultAsync())!;
        }

        public async Task<string> GetColor(string courseId)
        {
            return (await (from kw in _dbContext.Keywords
                where kw.CourseId.Equals(courseId)
                select kw.Color).FirstOrDefaultAsync())!;
        }

        public Task<string> GetColorBySubjectCode(string subjectCode)
        {
            throw new NotImplementedException();
        }

        public long Count(string discipline, string keyword1)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetCache(string courseId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Keyword>> GetKeywordsByDisciplineId(int disciplineId)
        {
            return await (from kw in _dbContext.Keywords
                where kw.DisciplineId == disciplineId
                orderby kw.Keyword1
                select kw).ToListAsync();
        }

        public void Insert(Keyword keyword)
        {
            _dbContext.Keywords!.Add(keyword);
        }

        public Task DeleteAllKeywords()
        {
            throw new NotImplementedException();
        }

        public Task<Keyword> GetByCourseId(int intCourseId)
        {
            throw new NotImplementedException();
        }

        public Task<long> Count()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Exists(string yearValue, string semesterValue)
        {
            return await (from cf in _dbContext.Courses
                where cf.YearValue.Equals(yearValue)
                      && cf.SemesterValue.Equals(semesterValue)
                select cf).AnyAsync();
        }

        public Course? Insert(Course? course)
        {
            Debug.Assert(course != null, nameof(course) + " != null");
            return _dbContext.Courses!.Add(course).Entity;
        }

        public Task<Course?> GetCourse(string yearValue, string semesterValue)
        {
            return (from cf in _dbContext.Courses
                where cf.YearValue.Equals(yearValue) && cf.SemesterValue.Equals(semesterValue)
                select cf).FirstAsync();
        }

        public Task<Course?> GetLatestCourse()
        {
            return (from c in _dbContext.Courses
                orderby c.CreatedDate descending
                select c).FirstOrDefaultAsync();
        }

        public Task<List<Keyword>> GetKeywordsByQuery(string? text, int limit)
        {
            IQueryable<Keyword> keywords;
            if (string.IsNullOrWhiteSpace(text))
            {
                keywords = from kw in _dbContext.Keywords
                    from ds in _dbContext.Disciplines
                    orderby ds.Name, kw.Keyword1, kw.SubjectName
                    where kw.DisciplineId == ds.Id
                    select kw;
            }
            else
            {
                text = text.Trim().ToLower().SuperCleanString();
                bool hasSpace = text.Contains(' ');
                string[] slices = text.Trim().ToLower().Split(' ');

                keywords = from kw in _dbContext.Keywords
                    from ds in _dbContext.Disciplines
                    where
                        // Keyword1 chứa đoạn text
                        kw.Keyword1.ToLower().Contains(text)
                        // Tên môn học bắt đầu bằng text
                        || kw.SubjectName.ToLower().StartsWith(text)
                        && kw.DisciplineId == ds.Id
                    orderby ds.Name, kw.Keyword1, kw.SubjectName
                    select kw;
            }

            return keywords
                .Include(kw => kw.Discipline)
                .Take(limit)
                .ToListAsync();
        }

        public void InsertAll(IEnumerable<Discipline?> disciplines)
        {
            _dbContext.Disciplines!.AddRange(disciplines!);
        }

        public async Task<List<Keyword>> GetAllKeyword()
        {
            return await _dbContext.Keywords.ToListAsync();
        }

        public void InsertAll(IEnumerable<Keyword> keywords)
        {
            _dbContext.Keywords!.AddRange(keywords);
        }

        public async Task<Discipline> GetDisciplineByName(string? name)
        {
            return await (from discipline in _dbContext.Disciplines
                where discipline.Name != null
                      && discipline.Name.Equals(name)
                select discipline).FirstAsync();
        }
    }
}