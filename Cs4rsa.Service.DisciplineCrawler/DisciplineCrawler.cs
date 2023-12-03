using Cs4rsa.Common;
using Cs4rsa.Service.DisciplineCrawler;
using Cs4rsa.Utils;

using HtmlAgilityPack;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Cs4rsa.Services.DisciplineCrawlerSvc.Crawlers
{
    public class DisciplineCrawler
    {
        private readonly IDisciplineHtmlGetter _htmlGetter;
        public DisciplineCrawler(IDisciplineHtmlGetter htmlGetter)
        {
            _htmlGetter = htmlGetter;
        }

        /// <summary>
        /// Cào data từ Course DTU.
        /// </summary>
        public List<Discipline> GetDisciplineAndKeyword(string currSemesterValue)
        {
            string URL = $"http://courses.duytan.edu.vn/Modules/academicprogram/CourseResultSearch.aspx?keyword2=*&scope=1&hocky={currSemesterValue}&t={CrawlUtils.GetTimeFromEpoch()}";

            HtmlDocument document = _htmlGetter.GetHtmlDocument(URL);
            IEnumerable<HtmlNode> trTags = document
                .DocumentNode
                .Descendants("tr")
                .Where(node => node.HasClass("lop"));

            string currentDiscipline = null;
            Discipline objDiscipline = null;
            List<Discipline> disciplines = new List<Discipline>();
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
                    objDiscipline = new Discipline
                    {
                        Name = discipline,
                        Keywords = new List<Keyword>()
                    };
                    disciplines.Add(objDiscipline);
                }

                if (discipline == currentDiscipline)
                {
                    string keyword1 = disciplineAndKeywordSplit[1];
                    HtmlNode subjectNameAnchorTag = tdTags[1].Element("a");
                    string subjectName = subjectNameAnchorTag.InnerText.Trim();
                    Keyword keyword = new Keyword()
                    {
                        Keyword1 = keyword1,
                        CourseId = courseId,
                        SubjectName = subjectName,
                    };
                    objDiscipline.Keywords.Add(keyword);
                }
            }
            return disciplines;
        }

        private static string GetCourseIdFromHref(string hrefValue)
        {
            string[] hrefValueSlides = hrefValue.Split('&');
            return hrefValueSlides[1].Split('=')[1];
        }
    }
}

