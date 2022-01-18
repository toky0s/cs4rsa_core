using DisciplineCrawlerDLL.Interfaces;
using DisciplineCrawlerDLL.Models;
using DisciplineCrawlerDLL.Utils;
using HelperService;
using HtmlAgilityPack;

namespace DisciplineCrawlerDLL.Crawlers
{
    public class DisciplineCrawler : IDisciplineCrawler
    {

        public DisciplineCrawler()
        {
        }

        public async Task<List<Discipline>> GetDisciplines(
            string currentSemesterValue)
        {
            string URL = $"http://courses.duytan.edu.vn/Modules/academicprogram/CourseResultSearch.aspx?keyword2=*&scope=1&hocky={currentSemesterValue}&t={Helpers.GetTimeFromEpoch()}";
            List<Discipline> disciplines = new();

            HtmlWeb htmlWeb = new();
            HtmlDocument document = await htmlWeb.LoadFromWebAsync(URL);
            HtmlNode[] trTags = document.DocumentNode.Descendants("tr").ToArray();
            trTags = trTags.Where(node => node.HasClass("lop")).ToArray();

            string currentDiscipline = "";
            int disciplineId = 0;
            foreach (HtmlNode trTag in trTags)
            {
                HtmlNode[] tdTags = trTag.Descendants("td").ToArray();
                HtmlNode disciplineAnchorTag = tdTags[0].Element("a");
                string courseId = GetCourseIdFromHref(disciplineAnchorTag.Attributes["href"].Value);
                string disciplineAndKeyword = disciplineAnchorTag.InnerText.Trim();
                string[] disciplineAndKeywordSplit = StringHelper.SplitAndRemoveAllSpace(disciplineAndKeyword);

                string discipline = disciplineAndKeywordSplit[0];


                if (currentDiscipline == "" || currentDiscipline != discipline)
                {
                    currentDiscipline = discipline;
                    disciplineId++;
                    Discipline newDiscipline = new(disciplineId, discipline);
                    disciplines.Add(newDiscipline);
                }

                if (discipline == currentDiscipline)
                {
                    string keyword1 = disciplineAndKeywordSplit[1];
                    ColorGen colorGen = new(disciplines);
                    string color = colorGen.GenerateColor();
                    HtmlNode subjectNameAnchorTag = tdTags[1].Element("a");
                    string subjectName = subjectNameAnchorTag.InnerText.Trim();
                    Keyword keyword = new(keyword1, int.Parse(courseId), subjectName, color);
                    Discipline? keywordDiscipline = disciplines
                        .Where(dl => dl.DisciplineId == disciplineId)
                        .FirstOrDefault();
                    if (keywordDiscipline != null)
                    {
                        keywordDiscipline.Keywords.Add(keyword);
                    }
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

