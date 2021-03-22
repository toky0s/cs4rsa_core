using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using HtmlAgilityPack;
using cs4rsa.Crawler;
using cs4rsa.Database;
using cs4rsa.Properties;

namespace cs4rsa.Crawler
{
    public class DisciplineKeywordInfo
    {
        private string courseId;
        private string subjectName;
        private string keyword1;
        private string discipline;

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

        public string Discipline
        {
            get
            {
                return discipline;
            }
        }
        public DisciplineKeywordInfo(string keyword1, string courseId, string subjectName, string discipline)
        {
            this.keyword1 = keyword1;
            this.courseId = courseId;
            this.subjectName = subjectName;
            this.discipline = discipline;
        }
    }

    class DisciplineData
    {
        public HomeCourseSearch homeCourseSearch = new HomeCourseSearch();
        public string DISCIPLINE_JSON_PATH = Helpers.Helpers.GetFilePathAtApp(Resources.cs4rsa_json_discipline_path);

        /// <summary>
        /// Lấy ra tên của môn môn học theo mã môn từ file JSON.
        /// </summary>
        /// <param name="subjectCode">Mã môn học, ví dụ: CS 414</param>
        /// <returns></returns>
        public string GetSubjectName(string subjectCode)
        {
            JObject jsonObject = JObject.Parse(File.ReadAllText(DISCIPLINE_JSON_PATH));
            var data = jsonObject.ToObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>();
            string discipline = Helpers.StringHelper.SplitAndRemoveAllSpace(subjectCode)[0];
            string keyword1 = Helpers.StringHelper.SplitAndRemoveAllSpace(subjectCode)[1];
            Dictionary<string, string> subjectData = data[discipline][keyword1];
            return subjectData["subject_name"];
        }

        /// <summary>
        /// Lấy ra courseId của môn môn học theo mã môn từ file JSON.
        /// </summary>
        /// <param name="subjectCode">Mã môn học, ví dụ: CS 414</param>
        /// <returns></returns>
        public string GetCourseId(string subjectCode)
        {
            JObject jsonObject = JObject.Parse(File.ReadAllText(DISCIPLINE_JSON_PATH));
            var data = jsonObject.ToObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>();
            string discipline = Helpers.StringHelper.SplitAndRemoveAllSpace(subjectCode)[0];
            string keyword1 = Helpers.StringHelper.SplitAndRemoveAllSpace(subjectCode)[1];
            Dictionary<string, string> subjectData = data[discipline][keyword1];
            return subjectData["course_id"];
        }


        /// <summary>
        /// Đọc file json chứa thông tin mã ngành sau đó trả về một List string chứa mã ngành.</string>
        /// </summary>
        /// <returns></returns>
        public List<string> GetDisciplines()
        {
            if (!File.Exists(DISCIPLINE_JSON_PATH))
            {
                DisciplineDictToJsonFile();
            }
            JObject jObject = JObject.Parse(File.ReadAllText(DISCIPLINE_JSON_PATH));
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
        public List<string> GetKeyword1s(string discipline)
        {
            JObject jObject = JObject.Parse(File.ReadAllText(DISCIPLINE_JSON_PATH));
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

        public List<DisciplineKeywordInfo> GetDisciplineKeywordInfos(string discipline)
        {
            JObject jObject = JObject.Parse(File.ReadAllText(DISCIPLINE_JSON_PATH));
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
                        DisciplineKeywordInfo disciplineKeywordInfo = new DisciplineKeywordInfo(keyword1, courseId, subjectName, discipline);
                        disciplineKeywordInfos.Add(disciplineKeywordInfo);
                    }
                    return disciplineKeywordInfos;
                }
            }
            return null;
        }

        /// <summary>
        /// Lấy ra thông tin của môt discipline truyền vào từ data đọc từ json chứa thông tin của các discipline.
        /// </summary>
        /// <param name="data">Data chứa thông tin của discipline được đọc từ JObject.</param>
        /// <param name="discipline">Mã ngành.</param>
        /// <returns></returns>
        private Dictionary<string, Dictionary<string, string>> GetInfoWithDisciplineFromJObject(Dictionary<string, Dictionary<string, string>> data, string discipline)
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


        private Dictionary<string, Dictionary<string, Dictionary<string, string>>> GetDisciplineSubjectJson()
        {
            string URL = String.Format(
                "http://courses.duytan.edu.vn/Modules/academicprogram/CourseResultSearch.aspx?keyword2=*&scope=1&hocky={0}&t={1}",
                homeCourseSearch.CurrentSemesterValue,
                Helpers.Helpers.GetTimeFromEpoch());

            HtmlWeb htmlWeb = new HtmlWeb();
            HtmlDocument document = htmlWeb.Load(URL);
            HtmlNode[] trTags = document.DocumentNode.Descendants("tr").ToArray();

            trTags = trTags.Where(node => node.HasClass("lop")).ToArray();
            var disciplines = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
            string currentDiscipline = null;
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
                    Dictionary<string, Dictionary<string, string>> disciplineKeywords = new Dictionary<string, Dictionary<string, string>>();
                    disciplines.Add(currentDiscipline, disciplineKeywords);
                }

                if (discipline == currentDiscipline)
                {
                    HtmlNode subjectNameAnchorTag = tdTags[1].Element("a");
                    string subjectName = subjectNameAnchorTag.InnerText.Trim();
                    Dictionary<string, string> subjectDatas = new Dictionary<string, string>();
                    subjectDatas.Add("course_id", courseId);
                    subjectDatas.Add("subject_name", subjectName);
                    subjectDatas.Add("discipline", discipline);
                    subjectDatas.Add("keyword1", keyword1);
                    disciplines[discipline].Add(keyword1, subjectDatas);
                }
            }
            return disciplines;
        }

        public void GetDisciplineAndKeywordDatabase()
        {
            Cs4rsaData cs4RsaData = new Cs4rsaData();
            string URL = String.Format(
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
                    cs4RsaData.AddDiscipline(currentDiscipline);
                }

                if (discipline == currentDiscipline)
                {
                    HtmlNode subjectNameAnchorTag = tdTags[1].Element("a");
                    string subjectName = subjectNameAnchorTag.InnerText.Trim();
                    cs4RsaData.AddKeyword(keyword1, courseId, disciplineId, subjectName);
                }
            }
        }

        public void DisciplineDictToJsonFile()
        {
            string json = JsonConvert.SerializeObject(GetDisciplineSubjectJson());
            File.WriteAllText(DISCIPLINE_JSON_PATH, json);
        }

        private string GetCourseIdFromHref(string hrefValue)
        {
            string[] hrefValueSlides = hrefValue.Split('&');
            return hrefValueSlides[1].Split('=')[1];
        }
    }
}

