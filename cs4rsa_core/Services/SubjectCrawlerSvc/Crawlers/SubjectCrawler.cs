using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;
using Cs4rsa.Services.CourseSearchSvc.Crawlers;
using Cs4rsa.Services.SubjectCrawlerSvc.Crawlers.Interfaces;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes;
using Cs4rsa.Services.TeacherCrawlerSvc.Crawlers.Interfaces;

using HtmlAgilityPack;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Cs4rsa.Services.SubjectCrawlerSvc.Crawlers
{
    public class SubjectCrawler : ISubjectCrawler
    {
        #region Services
        private readonly IUnitOfWork _unitOfWork;
        private readonly CourseCrawler _courseCrawler;
        private readonly ITeacherCrawler _teacherCrawler;
        private readonly HtmlWeb _htmlWeb;
        #endregion

        /// <summary>
        /// Bộ cào Subject từ Course DTU
        /// </summary>
        public SubjectCrawler(
            CourseCrawler courseCrawler,
            IUnitOfWork unitOfWork,
            ITeacherCrawler teacherCrawler,
            HtmlWeb htmlWeb
        )
        {
            _unitOfWork = unitOfWork;
            _courseCrawler = courseCrawler;
            _teacherCrawler = teacherCrawler;
            _htmlWeb = htmlWeb;
        }

        public async Task<Subject> Crawl(string discipline, string keyword1, bool isUseCache, bool withTeacher)
        {
            Keyword keyword = _unitOfWork.Keywords.GetKeyword(discipline, keyword1);
            if (isUseCache && keyword.Cache != null)
            {
                HtmlDocument htmlDocument = new();
                htmlDocument.LoadHtml(keyword.Cache);
                return await Crawl(htmlDocument, keyword.CourseId, withTeacher);
            }
            else
            {
                string semesterId = _courseCrawler.CurrentSemesterValue;
                string url = $"http://courses.duytan.edu.vn/Modules/academicprogram/CourseClassResult.aspx?courseid={keyword.CourseId}&semesterid={semesterId}&timespan={semesterId}";
                HtmlDocument htmlDocument = await _htmlWeb.LoadFromWebAsync(url);
                Subject subject = await Crawl(htmlDocument, keyword.CourseId, withTeacher);
                _unitOfWork.Keywords.UpdateCacheByKeywordID(keyword.KeywordId, subject.RawSoup);
                return subject;
            }
        }

        public async Task<string> CrawlCache(string courseId)
        {
            try
            {
                string semesterId = _courseCrawler.CurrentSemesterValue;
                string url = $"http://courses.duytan.edu.vn/Modules/academicprogram/CourseClassResult.aspx?courseid={courseId}&semesterid={semesterId}&timespan={semesterId}";
                HtmlDocument htmlDocument = await _htmlWeb.LoadFromWebAsync(url);
                Debug.Assert(IsSubjectExists(htmlDocument));
                return IsSubjectExists(htmlDocument) ? htmlDocument.DocumentNode.OuterHtml : null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] {ex.Message}");
                return null;
            }
        }

        public async Task<Subject> Crawl(int courseId, bool isUseCache, bool withTeacher)
        {
            Keyword keyword = _unitOfWork.Keywords.GetKeyword(courseId);
            if (keyword == null)
            {
                return null;
            }

            if (isUseCache && keyword.Cache != null)
            {
                HtmlDocument htmlDocument = new();
                htmlDocument.LoadHtml(keyword.Cache);
                return await Crawl(htmlDocument, keyword.CourseId, withTeacher);
            }
            else
            {
                string semesterId = _courseCrawler.CurrentSemesterValue;
                string url = $"http://courses.duytan.edu.vn/Modules/academicprogram/CourseClassResult.aspx?courseid={courseId}&semesterid={semesterId}&timespan={semesterId}";
                HtmlDocument htmlDocument = await _htmlWeb.LoadFromWebAsync(url);
                Subject subject = await Crawl(htmlDocument, keyword.CourseId, withTeacher);
                if (subject != null)
                {
                    _unitOfWork.Keywords.UpdateCacheByKeywordID(keyword.KeywordId, subject.RawSoup);
                }
                return subject;
            }
        }

        public async Task<Subject> Crawl(HtmlDocument htmlDocument, int courseId, bool withTeacher)
        {
            if (!IsSubjectExists(htmlDocument)) return null;

            HtmlNode table = htmlDocument.DocumentNode.Descendants("table").ToArray()[2];
            HtmlNode[] trTags = table.Descendants("tr").ToArray();
            string subjectCode = trTags[0].Elements("td").ToArray()[1].InnerText.Trim();

            string name = _unitOfWork.Keywords.GetKeyword(courseId).SubjectName;

            string studyUnit = trTags[1].Elements("td").ToArray()[1].GetDirectInnerText().Split(' ')[24];
            string studyUnitType = trTags[2].Elements("td").ToArray()[1].InnerText.Trim();
            string studyType = trTags[3].Elements("td").ToArray()[1].InnerText.Trim();
            string semester = trTags[4].Elements("td").ToArray()[1].InnerText.Trim();

            string mustStudySubject = trTags[5].Elements("td").ToArray()[1].InnerText.Trim();
            string parallelSubject = trTags[6].Elements("td").ToArray()[1].InnerText.Trim();

            string description = trTags[7].Elements("td").ToArray()[1].InnerText.Trim();

            string rawSoup = htmlDocument.DocumentNode.OuterHtml;
            return await Subject.CreateAsync(
                name,
                subjectCode,
                studyUnit,
                studyUnitType,
                studyType,
                semester,
                mustStudySubject,
                parallelSubject,
                description,
                rawSoup,
                courseId,
                _teacherCrawler,
                _unitOfWork,
                _htmlWeb,
                withTeacher
            );
        }

        private static bool IsSubjectExists(HtmlDocument htmlDocument)
        {
            HtmlNode span = htmlDocument.DocumentNode.SelectSingleNode("div[2]/span");
            if (span != null)
            {
                return false;
            }
            IEnumerable<HtmlNode> tables = htmlDocument.DocumentNode.Descendants("table");
            return tables.Any();
        }
    }
}
