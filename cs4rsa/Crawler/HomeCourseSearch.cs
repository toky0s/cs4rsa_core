using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using cs4rsa.Properties;

namespace cs4rsa.Crawler
{
    public class DisciplineKeywordInfo
    {
        private string courseId;
        private string subjectName;
        private string keyword1;
        public string CourseID
        {
            get
            {
                return courseId;
            }
            set
            {
                courseId = value;
            }
        }
        public string SubjectName
        {
            get
            {
                return subjectName;
            }
            set
            {
                subjectName = value;
            }
        }
        public string Keyword1
        {
            get
            {
                return keyword1;
            }
            set
            {
                keyword1 = value;
            }
        }
        public DisciplineKeywordInfo(string keyword1, string courseId, string subjectName)
        {
            this.keyword1 = keyword1;
            this.courseId = courseId;
            this.subjectName = subjectName;
        }
    }
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

        private static string DISCIPLINE_JSON_PATH = Helper.Helper.GetFilePathAtApp(Resources.cs4rsa_json_discipline_path);
        private static string DISCIPLINE_REFACTOR_JSON_PATH = Helper.Helper.GetFilePathAtApp(Resources.cs4rsa_refactor_json_discipline_path);

        public HomeCourseSearch()
        {
            HtmlWeb htmlWeb = new HtmlWeb();
            string URL_YEAR_COMBOBOX = "http://courses.duytan.edu.vn/Modules/academicprogram/ajax/LoadNamHoc.aspx?namhocname=cboNamHoc2&id=2";
            HtmlDocument document = htmlWeb.Load(URL_YEAR_COMBOBOX);
            currentYearValue = GetCurrentValue(document);
            currentYearInfo = GetCurrentInfo(document);

            string URL_SEMESTER_COMBOBOX = String.Format("http://courses.duytan.edu.vn/Modules/academicprogram/ajax/LoadHocKy.aspx?hockyname=cboHocKy1&namhoc={0}", currentYearValue);
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
            JObject jsonObject = JObject.Parse(File.ReadAllText(DISCIPLINE_JSON_PATH));
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
            JObject jsonObject = JObject.Parse(File.ReadAllText(DISCIPLINE_JSON_PATH));
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

        public static void RefactorDisciplineToJsonFile(string path)
        {
            string jsonData = GetRefactorJsonString(DISCIPLINE_JSON_PATH);
            File.WriteAllText(path, jsonData);
        }


        /// <summary>
        /// Đọc file json chứa thông tin mã ngành sau đó trả về một List string chứa mã ngành.</string>
        /// </summary>
        /// <returns></returns>
        public static List<string> GetDisciplines()
        {
            JObject jObject = JObject.Parse(File.ReadAllText(DISCIPLINE_REFACTOR_JSON_PATH));
            Dictionary<string, Dictionary<string, Dictionary<string, string>>> data = jObject.ToObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>();
            List<string> disciplines = new List<string>();
            foreach (string discipline in data.Keys.ToArray())
            {
                disciplines.Add(discipline);
            }
            return disciplines;
        }

        /// <summary>
        /// Lấy ra danh sách Keyword1 của Discipline truyền vào.
        /// </summary>
        /// <param name="discipline">Một chuỗi discipline.</param>
        /// <returns>List string keyword1.</returns>
        public static List<string> GetKeyword1s(string discipline)
        {
            JObject jObject = JObject.Parse(File.ReadAllText(DISCIPLINE_REFACTOR_JSON_PATH));
            Dictionary<string, Dictionary<string, Dictionary<string, string>>> data = jObject.ToObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>();
            List<string> keyword1s = new List<string>();
            foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, string>>> item in data)
            {
                if (item.Key == discipline)
                {
                    List<string> keyword1sInItem = item.Value.Keys.ToList();
                    keyword1s = keyword1s.Concat(keyword1sInItem).ToList();
                }
            }
            return keyword1s;
        }

        public static List<DisciplineKeywordInfo> GetDisciplineKeywordInfos(string discipline)
        {
            JObject jObject = JObject.Parse(File.ReadAllText(DISCIPLINE_REFACTOR_JSON_PATH));
            Dictionary<string, Dictionary<string, Dictionary<string, string>>> data = jObject.ToObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>();
            List<DisciplineKeywordInfo> disciplineKeywordInfos = new List<DisciplineKeywordInfo>();
            foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, string>>> item in data)
            {
                if (item.Key == discipline)
                {
                    foreach (KeyValuePair<string, Dictionary<string, string>> discipineItem in item.Value)
                    {
                        string keyword1 = discipineItem.Key;
                        string courseId = discipineItem.Value["course_id"];
                        string subjectName = discipineItem.Value["subject_name"];
                        DisciplineKeywordInfo disciplineKeywordInfo = new DisciplineKeywordInfo(keyword1, courseId, subjectName);
                        disciplineKeywordInfos.Add(disciplineKeywordInfo);
                    }
                    return disciplineKeywordInfos;
                }
            }
            return null;
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
            foreach (HtmlNode trTag in trTags)
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
                subjectDatas.Add("discipline", discipline.Split(' ')[0]);
                subjectDatas.Add("keyword1", discipline.Split(' ')[1]);
                disciplines.Add(discipline, subjectDatas);
            }
            return JsonConvert.SerializeObject(disciplines);
        }

        private string GetCourseIdFromHref(string hrefValue)
        {
            string[] hrefValueSlides = hrefValue.Split('&');
            return hrefValueSlides[1].Split('=')[1];
        }

        /// <summary>
        /// Đưa về dạng Json tối ưu cho discipline và keyword1.
        /// </summary>
        /// <param name="path">Đường dẫn tới file Json cũ.</param>
        private static string GetRefactorJsonString(string path)
        {
            JObject jsonObject = JObject.Parse(File.ReadAllText(path));
            Dictionary<string, Dictionary<string, string>> data = jsonObject.ToObject<Dictionary<string, Dictionary<string, string>>>();
            Dictionary<string, Dictionary<string, Dictionary<string, string>>> refactorJson = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

            List<string> disciplines = HomeCourseSearch.GetDisciplines();
            foreach (string item in disciplines)
            {
                var infoDiscipline = GetInfoWithDisciplineFromJObject(data, item);
                refactorJson.Add(item, infoDiscipline);
            }
            return JsonConvert.SerializeObject(refactorJson);
        }

        /// <summary>
        /// Lấy ra thông tin của môt discipline truyền vào từ data đọc từ json chứa thông tin của các discipline.
        /// </summary>
        /// <param name="data">Data chứa thông tin của discipline được đọc từ JObject.</param>
        /// <param name="discipline">Mã ngành.</param>
        /// <returns></returns>
        private static Dictionary<string, Dictionary<string, string>> GetInfoWithDisciplineFromJObject(Dictionary<string, Dictionary<string, string>> data, string discipline)
        {
            Dictionary<string, Dictionary<string, string>> infoDiscipline = new Dictionary<string, Dictionary<string, string>>();
            foreach (KeyValuePair<string, Dictionary<string, string>> item in data)
            {
                if (item.Value["discipline"] == discipline)
                {
                    Dictionary<string, string> disciplineKeywordData = new Dictionary<string, string>()
                    {
                        { "course_id", item.Value["course_id"]},
                        { "subject_name" , item.Value["subject_name"] }
                    };
                    infoDiscipline.Add(item.Value["keyword1"], disciplineKeywordData);
                }
            }
            return infoDiscipline;
        }
    }
}

