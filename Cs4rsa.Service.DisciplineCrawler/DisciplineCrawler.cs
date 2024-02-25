using System.Collections.Generic;
using System.Linq;
using Cs4rsa.Common;
using Cs4rsa.Infrastructure.Common;

namespace Cs4rsa.Service.DisciplineCrawler
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
            var url = $"http://courses.duytan.edu.vn/Modules/academicprogram/CourseResultSearch.aspx?keyword2=*&scope=1&hocky={currSemesterValue}&t={CrawlUtils.GetTimeFromEpoch()}";

            var document = _htmlGetter.GetHtmlDocument(url);
            var trTags = document
                .DocumentNode
                .Descendants("tr")
                .Where(node => node.HasClass("lop"));

            string currentDiscipline = null;
            Discipline objDiscipline = null;
            var disciplines = new List<Discipline>();
            foreach (var trTag in trTags)
            {
                var tdTags = trTag.Descendants("td").ToArray();
                var disciplineAnchorTag = tdTags[0].Element("a");
                var courseId = GetCourseIdFromHref(disciplineAnchorTag.Attributes["href"].Value);
                var disciplineAndKeyword = disciplineAnchorTag.InnerText.Trim();
                var disciplineAndKeywordSplit = disciplineAndKeyword.SplitAndRemoveAllSpace();
                var discipline = disciplineAndKeywordSplit[0];

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
                    var keyword1 = disciplineAndKeywordSplit[1];
                    var subjectNameAnchorTag = tdTags[1].Element("a");
                    var subjectName = subjectNameAnchorTag.InnerText.Trim();
                    var keyword = new Keyword()
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
            var hrefValueSlides = hrefValue.Split('&');
            return hrefValueSlides[1].Split('=')[1];
        }
    }
}

