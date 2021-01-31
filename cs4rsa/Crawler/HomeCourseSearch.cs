using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using cs4rsa.BasicData;
using cs4rsa.Helper;

namespace cs4rsa.Crawler
{
    /// <summary>
    /// <para>Class này bao gồm các method liên quan đến tìm kiếm thông tin Học Kỳ, Năm học và Thông tin môn học được Crawl từ web.</para>
    /// </summary>
    public class HomeCourseSearch
    {
        private static string currentYearValue;
        private static string currentYearInfo;
        private static string currentSemesterValue;
        private static string currentSemesterInfo;

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

        private const string DISCIPLINES_JSON_FILE_NAME = "cs4rsa_disciplines.json";

        public HomeCourseSearch()
        {
            HtmlWeb htmlWeb = new HtmlWeb();
            string URL_YEAR_COMBOBOX = "http://courses.duytan.edu.vn/Modules/academicprogram/ajax/LoadNamHoc.aspx?namhocname=cboNamHoc2&id=2";
            HtmlDocument document = htmlWeb.Load(URL_YEAR_COMBOBOX);
            currentYearValue = GetCurrentValue(document);
            currentYearInfo = GetCurrentInfo(document);

            string URL_SEMESTER_COMBOBOX = String.Format("http://courses.duytan.edu.vn/Modules/academicprogram/ajax/LoadHocKy.aspx?hockyname=cboHocKy1&namhoc={0}",currentYearValue);
            document = htmlWeb.Load(URL_SEMESTER_COMBOBOX);
            currentSemesterValue = GetCurrentValue(document);
            currentSemesterInfo = GetCurrentInfo(document);
        }

        /// <summary>
        /// Lấy ra tên của môn môn học theo mã môn từ file JSON.
        /// </summary>
        /// <param name="subjectCode">Mã môn học, ví dụ: CS 414</param>
        /// <returns></returns>
        public static string GetSubjectName(string subjectCode)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"\" + DISCIPLINES_JSON_FILE_NAME;
            JObject jsonObject = JObject.Parse(File.ReadAllText(path));
            Dictionary<string, Dictionary<string, string>> data = jsonObject.ToObject<Dictionary<string, Dictionary<string, string>>>();
            Dictionary<string, string> subjectData = data[subjectCode];
            return subjectData["subject_name"];
        }

        /// <summary>
        /// Lấy ra courseId của môn môn học theo mã môn từ file JSON.
        /// </summary>
        /// <param name="subjectCode">Mã môn học, ví dụ: CS 414</param>
        /// <returns></returns>
        public static string GetCourseId(string subjectCode)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"\" + DISCIPLINES_JSON_FILE_NAME;
            JObject jsonObject = JObject.Parse(File.ReadAllText(path));
            Dictionary<string, Dictionary<string, string>> data = jsonObject.ToObject<Dictionary<string, Dictionary<string, string>>>();
            Dictionary<string, string> subjectData = data[subjectCode];
            return subjectData["course_id"];
        }

        /// <summary>
        /// Lưu file Discipline JSON.
        /// </summary>
        /// <param name="path">Đường dẫn.</param>
        public void DisciplineDatasToJsonFile(string path)
        {
            string jsonData = GetDisciplineSubjectJson();
            File.WriteAllText(path, jsonData);
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
                Helper.Helper.getTimeFromEpoch());

            HtmlWeb htmlWeb = new HtmlWeb();
            HtmlDocument document = htmlWeb.Load(URL);
            HtmlNode[] trTags = document.DocumentNode.Descendants("tr").ToArray();
            trTags = trTags.Where(node => node.HasClass("lop")).ToArray();

            Dictionary<string, Dictionary<string, string>> disciplines = new Dictionary<string, Dictionary<string, string>>();
            foreach(HtmlNode trTag in trTags)
            {
                HtmlNode[] tdTags = trTag.Descendants("td").ToArray();

                HtmlNode disciplineAnchorTag = tdTags[0].Element("a");
                string courseId = GetCourseIdFromHref(disciplineAnchorTag.Attributes["href"].Value);
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

        private string GetCourseIdFromHref(string hrefValue)
        {
            string[] hrefValueSlides = hrefValue.Split('&');
            return hrefValueSlides[1].Split('=')[1];
        }
    }
}
