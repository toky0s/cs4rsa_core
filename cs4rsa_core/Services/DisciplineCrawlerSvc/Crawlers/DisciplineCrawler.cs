using Cs4rsa.BaseClasses;
using Cs4rsa.Cs4rsaDatabase.DataProviders;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Services.CourseSearchSvc.Crawlers;
using Cs4rsa.Utils;

using HtmlAgilityPack;

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Cs4rsa.Services.DisciplineCrawlerSvc.Crawlers
{
    public class DisciplineCrawler : BaseCrawler
    {
        private readonly CourseCrawler _homeCourseSearch;
        private readonly IUnitOfWork _unitOfWork;
        private readonly HtmlWeb _htmlWeb;

        public DisciplineCrawler(
            CourseCrawler courseCrawler,
            IUnitOfWork unitOfWork,
            HtmlWeb htmlWeb
        )
        {
            _homeCourseSearch = courseCrawler;
            _unitOfWork = unitOfWork;
            _htmlWeb = htmlWeb;
        }

        /// <summary>
        /// Cào data từ Course DTU và lưu vào database.
        /// </summary>
        public int GetDisciplineAndKeyword(
            BackgroundWorker backgroundWorker,
            int numberOfSubjects
        )
        {
            int result = 0;
            float reportValue = 0;
            float jump = 0;
            if (numberOfSubjects != 0)
            {
                jump = 1000 / numberOfSubjects;
                reportValue = jump;
            }

            StringBuilder sbDiscipline = new();
            sbDiscipline.AppendLine("INSERT INTO Disciplines");
            sbDiscipline.AppendLine("VALUES");

            StringBuilder sbKeyword = new();
            sbKeyword.AppendLine("INSERT INTO Keywords");
            sbKeyword.AppendLine("VALUES");

            _unitOfWork.Keywords.DeleteAll();
            _unitOfWork.Disciplines.DeleteAll();

            string URL = $"http://courses.duytan.edu.vn/Modules/academicprogram/CourseResultSearch.aspx?keyword2=*&scope=1&hocky={_homeCourseSearch.CurrentSemesterValue}&t={GetTimeFromEpoch()}";

            HtmlDocument document = _htmlWeb.Load(URL);
            IEnumerable<HtmlNode> trTags = document.DocumentNode
                .Descendants("tr")
                .Where(node => node.HasClass("lop"));

            string currentDiscipline = null;
            int disciplineId = 0;
            int keywordId = 1;
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
                    // Build discipline insert query
                    sbDiscipline.AppendLine($"({disciplineId}, '{discipline}'),");
                }

                if (discipline == currentDiscipline)
                {
                    string keyword1 = disciplineAndKeywordSplit[1];
                    string color = ColorGenerator.GenerateColor();
                    HtmlNode subjectNameAnchorTag = tdTags[1].Element("a");
                    string subjectName = subjectNameAnchorTag.InnerText.Trim();
                    // Build keyword insert query
                    sbKeyword.AppendLine($"({keywordId}, '{keyword1}', {courseId}, '{subjectName}', '{color}', NULL, {disciplineId}),");
                    keywordId++;
                    result++;
                }

                // report work progress
                if (backgroundWorker != null)
                {
                    backgroundWorker.ReportProgress((int)reportValue);
                    reportValue += jump;
                }
            }

            #region Remove Commas
            sbDiscipline.RemoveLastCharAfterAppendLine();
            sbKeyword.RemoveLastCharAfterAppendLine();
            sbDiscipline.Append(';');
            sbKeyword.Append(';');
            #endregion

            #region Execute Command
            RawSql.ExecNonQuery(sbDiscipline.ToString());
            RawSql.ExecNonQuery(sbKeyword.ToString());
            #endregion

            return result;
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
            string URL = $"http://courses.duytan.edu.vn/Modules/academicprogram/CourseResultSearch.aspx?keyword2=*&scope=1&hocky={_homeCourseSearch.CurrentSemesterValue}&t={GetTimeFromEpoch()}";
            HtmlDocument document = _htmlWeb.Load(URL);

            HtmlNode node = document.DocumentNode;
            HtmlNode result = node.SelectSingleNode("/div/table/thead/tr/th");
            string innerText = result.InnerText;
            Regex numberMatchingRegex = new("([0-9][0-9][0-9])");
            Match match = numberMatchingRegex.Match(innerText);
            return int.Parse(match.Value);
        }
    }
}

