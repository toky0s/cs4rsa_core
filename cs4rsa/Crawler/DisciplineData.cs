using cs4rsa.Database;
using HtmlAgilityPack;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace cs4rsa.Crawler
{
    public class DisciplineData
    {
        public HomeCourseSearch homeCourseSearch = HomeCourseSearch.GetInstance();

        /// <summary>
        /// Cào data từ Course DTU và lưu vào database.
        /// </summary>
        public void GetDisciplineAndKeywordDatabase(BackgroundWorker backgroundWorker=null, double numberOfSubjects =0)
        {
            double reportValue = 0;
            double jump = 0;
            if (numberOfSubjects != 0)
            {
                jump = 1000 / numberOfSubjects;
                reportValue = jump;
            }
                
            Cs4rsaData cs4RsaData = new Cs4rsaData();
            string URL = string.Format(
                "http://courses.duytan.edu.vn/Modules/academicprogram/CourseResultSearch.aspx?keyword2=*&scope=1&hocky={0}&t={1}",
                homeCourseSearch.CurrentSemesterValue,
                Helpers.Helpers.GetTimeFromEpoch());

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
                string[] disciplineAndKeywordSplit = Helpers.StringHelper.SplitAndRemoveAllSpace(disciplineAndKeyword);

                string discipline = disciplineAndKeywordSplit[0];
                string keyword1 = disciplineAndKeywordSplit[1];

                if (currentDiscipline == null || currentDiscipline != discipline)
                {
                    currentDiscipline = discipline;
                    disciplineId++;
                    Cs4rsaDataEdit.AddDiscipline(currentDiscipline);
                }

                if (discipline == currentDiscipline)
                {
                    string color = ColorGenerator.GenerateColor();
                    HtmlNode subjectNameAnchorTag = tdTags[1].Element("a");
                    string subjectName = subjectNameAnchorTag.InnerText.Trim();
                    Cs4rsaDataEdit.AddKeyword(keyword1, courseId, disciplineId, subjectName, color);
                }

                // report work progress
                if (backgroundWorker != null)
                {
                    backgroundWorker.ReportProgress((int)reportValue);
                    reportValue += jump;
                }
            }
        }

        private string GetCourseIdFromHref(string hrefValue)
        {
            string[] hrefValueSlides = hrefValue.Split('&');
            return hrefValueSlides[1].Split('=')[1];
        }


        /// <summary>
        /// Lấy ra số lượng môn học hiện có trong học kỳ hiện tại.
        /// </summary>
        /// <returns>Số lượng môn học hiện có.</returns>
        public static int GetNumberOfSubjects()
        {
            HomeCourseSearch homeCourseSearch = HomeCourseSearch.GetInstance();
            string URL = string.Format(
            "http://courses.duytan.edu.vn/Modules/academicprogram/CourseResultSearch.aspx?keyword2=*&scope=1&hocky={0}&t={1}",
            homeCourseSearch.CurrentSemesterValue,
            Helpers.Helpers.GetTimeFromEpoch());

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

