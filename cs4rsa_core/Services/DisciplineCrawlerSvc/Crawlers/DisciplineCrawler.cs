using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;
using Cs4rsa.Services.CourseSearchSvc.Crawlers.Interfaces;
using Cs4rsa.Utils;

using HtmlAgilityPack;

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace Cs4rsa.Services.DisciplineCrawlerSvc.Crawlers
{
    public class DisciplineCrawler
    {
        private readonly ICourseCrawler _homeCourseSearch;
        private readonly IUnitOfWork _unitOfWork;
        private readonly HtmlWeb _htmlWeb;

        public DisciplineCrawler(
            ICourseCrawler courseCrawler,
            IUnitOfWork unitOfWork,
            ColorGenerator colorGenerator,
            HtmlWeb htmlWeb)
        {
            _homeCourseSearch = courseCrawler;
            _unitOfWork = unitOfWork;
            _htmlWeb = htmlWeb;
        }

        /// <summary>
        /// Cào data từ Course DTU và lưu vào database.
        /// </summary>
        public void GetDisciplineAndKeyword(
            BackgroundWorker backgroundWorker = null,
            short numberOfSubjects = 0)
        {
            float reportValue = 0;
            float jump = 0;
            if (numberOfSubjects != 0)
            {
                jump = 1000 / numberOfSubjects;
                reportValue = jump;
            }

            string URL = $"http://courses.duytan.edu.vn/Modules/academicprogram/CourseResultSearch.aspx?keyword2=*&scope=1&hocky={_homeCourseSearch.GetCurrentSemesterValue()}&t={Helpers.GetTimeFromEpoch()}";

            HtmlDocument document = _htmlWeb.Load(URL);
            IEnumerable<HtmlNode> trTags = document.DocumentNode
                .Descendants("tr")
                .Where(node => node.HasClass("lop"));

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
                    string color = ColorGenerator.GenerateColor();
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
        public short GetNumberOfSubjects()
        {
            string URL = $"http://courses.duytan.edu.vn/Modules/academicprogram/CourseResultSearch.aspx?keyword2=*&scope=1&hocky={_homeCourseSearch.GetCurrentSemesterValue()}&t={Helpers.GetTimeFromEpoch()}";
            HtmlDocument document = _htmlWeb.Load(URL);

            HtmlNode node = document.DocumentNode;
            HtmlNode result = node.SelectSingleNode("/div/table/thead/tr/th");
            string innerText = result.InnerText;
            Regex numberMatchingRegex = new("([0-9][0-9][0-9])");
            Match match = numberMatchingRegex.Match(innerText);
            return short.Parse(match.Value);
        }
    }
}

