using cs4rsa_core.Cs4rsaDatabase.Interfaces;
using cs4rsa_core.Cs4rsaDatabase.Models;
using cs4rsa_core.Services.CourseSearchSvc.Crawlers.Interfaces;
using cs4rsa_core.Services.SubjectCrawlerSvc.Crawlers.Interfaces;
using cs4rsa_core.Services.SubjectCrawlerSvc.DataTypes;
using cs4rsa_core.Utils.Interfaces;

using HtmlAgilityPack;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cs4rsa_core.Services.SubjectCrawlerSvc.Crawlers
{
    public class SubjectCrawler : ISubjectCrawler
    {
        #region Services
        private readonly IFolderManager _folderManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICourseCrawler _courseCrawler;
        #endregion

        /// <summary>
        /// Bộ cào Subject từ Course DTU
        /// </summary>
        /// <param name="discipline">Hai chữ cái đầu của mã môn (CS).</param>
        /// <param name="keyword1">Các chữ số đằng sau (414).</param>
        public SubjectCrawler(
            ICourseCrawler courseCrawler,
            IUnitOfWork unitOfWork,
            IFolderManager folderManager
        )
        {
            _unitOfWork = unitOfWork;
            _courseCrawler = courseCrawler;
            _folderManager = folderManager;
        }

        public async Task<Subject> Crawl(string discipline, string keyword1, bool isUseCache = true)
        {
            Keyword keyword = await _unitOfWork.Keywords.GetKeyword(discipline, keyword1);
            if (isUseCache && keyword.Cache != null)
            {
                HtmlDocument htmlDocument = new();
                htmlDocument.LoadHtml(keyword.Cache);
                return await Crawl(htmlDocument, keyword.CourseId);
            }
            else
            {
                Subject subject = await Crawl(keyword.CourseId);
                await SaveCache(keyword.KeywordId, subject.RawSoup);
                return subject;
            }
        }

        public async Task<Subject> Crawl(int courseId, bool isUseCache = true)
        {
            string semesterId = _courseCrawler.GetCurrentSemesterValue();
            string url = $"http://courses.duytan.edu.vn/Modules/academicprogram/CourseClassResult.aspx?courseid={courseId}&semesterid={semesterId}&timespan={semesterId}";
            HtmlWeb htmlWeb = new();
            HtmlDocument htmlDocument = await htmlWeb.LoadFromWebAsync(url);
            return await Crawl(htmlDocument, courseId);
        }

        public async Task<Subject> Crawl(HtmlDocument htmlDocument, int courseId)
        {
            if (IsSubjectExists(htmlDocument))
            {
                HtmlNode table = htmlDocument.DocumentNode.Descendants("table").ToArray()[2];
                HtmlNode[] trTags = table.Descendants("tr").ToArray();
                string subjectCode = trTags[0].Elements("td").ToArray()[1].InnerText.Trim();

                string name = (await _unitOfWork.Keywords.GetKeyword(courseId)).SubjectName;

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
                    _unitOfWork,
                    _folderManager);
            }
            return null;
        }

        public async Task SaveCache(int keywordId, string htmlRaw)
        {
            await _unitOfWork.BeginTransAsync();
            Keyword keyword = await _unitOfWork.Keywords.GetByIdAsync(keywordId);
            keyword.Cache = htmlRaw;
            _unitOfWork.Keywords.Update(keyword);
            await _unitOfWork.CompleteAsync();
            await _unitOfWork.CommitAsync();
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
