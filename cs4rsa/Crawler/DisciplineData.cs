using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;
using System.IO;
using HtmlAgilityPack;
using cs4rsa.Crawler;
using cs4rsa.Database;
using cs4rsa.Properties;

namespace cs4rsa.Crawler
{
    class DisciplineData
    {
        public HomeCourseSearch homeCourseSearch = HomeCourseSearch.GetInstance();

        public void GetDisciplineAndKeywordDatabase()
        {
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
            }
        }

        private string GetCourseIdFromHref(string hrefValue)
        {
            string[] hrefValueSlides = hrefValue.Split('&');
            return hrefValueSlides[1].Split('=')[1];
        }
    }
}

