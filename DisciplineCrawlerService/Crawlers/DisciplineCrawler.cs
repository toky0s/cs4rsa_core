using CourseSearchService.Crawlers.Interfaces;
using Cs4rsaDatabaseService.DataProviders;
using Cs4rsaDatabaseService.Interfaces;
using Cs4rsaDatabaseService.Models;
using HelperService;
using HtmlAgilityPack;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DisciplineCrawlerService.Crawlers
{
    public class DisciplineCrawler
    {
        private readonly ICourseCrawler _homeCourseSearch;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ColorGenerator _colorGenerator;

        public DisciplineCrawler(ICourseCrawler courseCrawler, IUnitOfWork unitOfWork, ColorGenerator colorGenerator)
        {
            _homeCourseSearch = courseCrawler;
            _unitOfWork = unitOfWork;
            _colorGenerator = colorGenerator;
        }

        /// <summary>
        /// Cào data từ Course DTU và lưu vào database.
        /// </summary>
        public void GetDisciplineAndKeyword(BackgroundWorker backgroundWorker = null, double numberOfSubjects = 0)
        {
            double reportValue = 0;
            double jump = 0;
            if (numberOfSubjects != 0)
            {
                jump = 1000 / numberOfSubjects;
                reportValue = jump;
            }

            string URL = $"http://courses.duytan.edu.vn/Modules/academicprogram/CourseResultSearch.aspx?keyword2=*&scope=1&hocky={_homeCourseSearch.GetCurrentSemesterValue()}&t={Helpers.GetTimeFromEpoch()}";

            HtmlWeb htmlWeb = new HtmlWeb();
            HtmlDocument document = htmlWeb.Load(URL);
            HtmlNode[] trTags = document.DocumentNode.Descendants("tr").ToArray();

            trTags = trTags.Where(node => node.HasClass("lop")).ToArray();

            string currentDiscipline = null;
            int disciplineId = 0;
            foreach (HtmlNode trTag in trTags)
            {
                HtmlNode[] tdTags = trTag.Descendants("td").ToArray();

                HtmlNode disciplineAnchorTag = tdTags[0].Element("a");
                string courseId = GetCourseIdFromHref(disciplineAnchorTag.Attributes["href"].Value);
                string disciplineAndKeyword = disciplineAnchorTag.InnerText.Trim();
                string[] disciplineAndKeywordSplit = StringHelper.SplitAndRemoveAllSpace(disciplineAndKeyword);

                string discipline = disciplineAndKeywordSplit[0];
                

                if (currentDiscipline == null || currentDiscipline != discipline)
                {
                    currentDiscipline = discipline;
                    disciplineId++;
                    Discipline discipline1 = new() { DisciplineId = disciplineId, Name = discipline };
                    _unitOfWork.Disciplines.Add(discipline1);
                    _unitOfWork.Complete();
                }

                if (discipline == currentDiscipline)
                {
                    string keyword1 = disciplineAndKeywordSplit[1];
                    string color = _colorGenerator.GenerateColor();
                    HtmlNode subjectNameAnchorTag = tdTags[1].Element("a");
                    string subjectName = subjectNameAnchorTag.InnerText.Trim();
                    Keyword keyword = new()
                    {
                        Keyword1 = keyword1,
                        CourseId = int.Parse(courseId),
                        DisciplineId = disciplineId,
                        SubjectName = subjectName,
                        Color = color
                    };
                    _unitOfWork.Keywords.Add(keyword);
                    _unitOfWork.Complete();
                }

                // report work progress
                if (backgroundWorker != null)
                {
                    backgroundWorker.ReportProgress((int)reportValue);
                    reportValue += jump;
                }
            }
        }

        private static string GetCourseIdFromHref(string hrefValue)
        {
            string[] hrefValueSlides = hrefValue.Split('&');
            return hrefValueSlides[1].Split('=')[1];
        }


        /// <summary>
        /// Lấy ra số lượng môn học hiện có trong học kỳ hiện tại.
        /// </summary>
        /// <returns>Số lượng môn học hiện có.</returns>
        public int GetNumberOfSubjects()
        {
            string URL = $"http://courses.duytan.edu.vn/Modules/academicprogram/CourseResultSearch.aspx?keyword2=*&scope=1&hocky={_homeCourseSearch.GetCurrentSemesterValue()}&t={Helpers.GetTimeFromEpoch()}";

            HtmlWeb htmlWeb = new HtmlWeb();
            HtmlDocument document = htmlWeb.Load(URL);

            HtmlNode node = document.DocumentNode;
            HtmlNode result = node.SelectSingleNode("/div/table/thead/tr/th");
            string innerText = result.InnerText;
            Regex numberMatchingRegex = new Regex("([0-9][0-9][0-9])");
            Match match = numberMatchingRegex.Match(innerText);
            return int.Parse(match.Value);
        }
    }
}

