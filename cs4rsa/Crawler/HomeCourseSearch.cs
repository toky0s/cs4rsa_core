using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System.IO;

namespace cs4rsa.Crawler
{
    class HomeCourseSearch
    {
        private readonly string currentYearValue;
        private readonly string currentYearInfo;
        private readonly string currentSemesterValue;
        private readonly string currentSemesterInfo;

        
        public HomeCourseSearch()
        {
            HtmlWeb htmlWeb = new HtmlWeb();
            string URL_YEAR_COMBOBOX = "http://courses.duytan.edu.vn/Modules/academicprogram/ajax/LoadNamHoc.aspx?namhocname=cboNamHoc2&id=2";
            HtmlDocument document = htmlWeb.Load(URL_YEAR_COMBOBOX);
            currentYearValue = getCurrentValue(document);
            currentYearInfo = getCurrentInfo(document);

            string URL_SEMESTER_COMBOBOX = String.Format("http://courses.duytan.edu.vn/Modules/academicprogram/ajax/LoadHocKy.aspx?hockyname=cboHocKy1&namhoc={0}",currentYearValue);
            document = htmlWeb.Load(URL_SEMESTER_COMBOBOX);
            currentSemesterValue = getCurrentValue(document);
            currentSemesterInfo = getCurrentInfo(document);
        }

        public string CurrentYearValue
        {
            get { return currentYearValue; }
        }

        public string CurrentYearInfo
        {
            get { return currentYearInfo; }
        }

        public string CurrentSemesterValue
        {
            get { return currentSemesterValue; }
        }

        public string CurrentSemesterInfo
        {
            get { return currentSemesterInfo; }
        }

        private string GetTime()
        {
            long fromUnixEpoche = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
            return fromUnixEpoche.ToString();
        }

        private string GetCurrentValue(HtmlDocument document)
        {
            var optionElements = document.DocumentNode.Descendants().Where(n => n.Name == "option");
            return optionElements.Last().Attributes["value"].Value;
        }

        private string GetCurrentInfo(HtmlDocument document)
        {
            var optionElements = document.DocumentNode.Descendants().Where(node => node.Name == "option");
            return optionElements.Last().InnerText.Trim();
        }

        private string GetDisciplineSubjectJson()
        {
            string URL = String.Format(
                "http://courses.duytan.edu.vn/Modules/academicprogram/CourseResultSearch.aspx?keyword2=*&scope=1&hocky={0}&t={1}", 
                currentSemesterValue, 
                GetTime());

            HtmlWeb htmlWeb = new HtmlWeb();
            HtmlDocument document = htmlWeb.Load(URL);
            HtmlNode[] trTags = document.DocumentNode.Descendants("tr").ToArray();
            trTags = trTags.Where(node => node.HasClass("lop")).ToArray();

            Dictionary<string, Dictionary<string, string>> disciplines = new Dictionary<string, Dictionary<string, string>>();
            foreach(HtmlNode trTag in trTags)
            {
                HtmlNode[] tdTags = trTag.Descendants("td").ToArray();

                HtmlNode disciplineAnchorTag = tdTags[0].Element("a");
                string courseId = GetCourseId(disciplineAnchorTag.Attributes["href"].Value);
                string discipline = disciplineAnchorTag.InnerText.Trim();

                HtmlNode subjectNameAnchorTag = tdTags[1].Element("a");
                string subjectName = subjectNameAnchorTag.InnerText.Trim();

                Dictionary<string, string> subjectDatas = new Dictionary<string, string>();
                subjectDatas.Add("course_id", courseId);
                subjectDatas.Add("subject_name", subjectName);
                disciplines.Add(discipline, subjectDatas);
            }
            return JsonConvert.SerializeObject(disciplines);
        }

        public void DisciplineDatasToJsonFile(string path)
        {
            string jsonData = getDisciplineSubjectJson();
            File.WriteAllText(path, jsonData);
            Console.WriteLine("OK");
        }

        private string GetCourseId(string hrefValue)
        {
            string[] hrefValueSlides = hrefValue.Split('&');
            return hrefValueSlides[1].Split('=')[1];
        }


    }
}
