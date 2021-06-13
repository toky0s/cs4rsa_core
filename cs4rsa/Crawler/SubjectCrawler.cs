using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.BasicData;
using HtmlAgilityPack;
using cs4rsa.Crawler;
using cs4rsa.Database;


namespace cs4rsa.Crawler
{
    public class SubjectCrawler
    {

        private HomeCourseSearch homeCourseSearch = HomeCourseSearch.GetInstance();
        private string subjectCode;
        public string SubjectCode
        {
            get { return subjectCode; }
            set
            {
                subjectCode = value;
            }
        }

        private string discipline;
        private string keyword1;

        /// <summary>
        /// Get a Subject of DTU.
        /// </summary>
        /// <param name="discipline">Hai chữ cái đầu của mã môn (CS).</param>
        /// <param name="keyword1">Các chữ số đằng sau (414).</param>
        public SubjectCrawler(string discipline, string keyword1)
        {
            subjectCode = discipline + " " + keyword1;
            this.discipline = discipline;
            this.keyword1 = keyword1;
        }

        public Subject ToSubject()
        {
            string courseId = Cs4rsaDataView.GetSingleDisciplineKeywordModel(discipline, keyword1).CourseID;
            string semesterId = homeCourseSearch.CurrentSemesterValue;

            string url = string.Format("http://courses.duytan.edu.vn/Modules/academicprogram/CourseClassResult.aspx?courseid={0}&semesterid={1}&timespan={2}", courseId, semesterId, semesterId);
            HtmlWeb htmlWeb = new HtmlWeb();
            HtmlDocument htmlDocument = htmlWeb.Load(url);

            // kiểm tra sự tồn tại của môn học
            HtmlNode span = htmlDocument.DocumentNode.SelectSingleNode("div[2]/span");
            if (span == null)
            {
                HtmlNode table = htmlDocument.DocumentNode.Descendants("table").ToArray()[2];
                HtmlNode[] trTags = table.Descendants("tr").ToArray();
                string subjectCode = trTags[0].Elements("td").ToArray()[1].InnerText.Trim();
                string name = Cs4rsaDataView.GetSingleDisciplineKeywordModel(discipline, keyword1).SubjectName;

                string studyUnit = trTags[1].Elements("td").ToArray()[1].GetDirectInnerText().Split(' ')[24];
                string studyUnitType = trTags[2].Elements("td").ToArray()[1].InnerText.Trim();
                string studyType = trTags[3].Elements("td").ToArray()[1].InnerText.Trim();
                string semester = trTags[4].Elements("td").ToArray()[1].InnerText.Trim();

                string mustStudySubject = trTags[5].Elements("td").ToArray()[1].InnerText.Trim();
                string parallelSubject = trTags[6].Elements("td").ToArray()[1].InnerText.Trim();

                string description = trTags[7].Elements("td").ToArray()[1].InnerText.Trim();

                string rawSoup = htmlDocument.DocumentNode.OuterHtml;
                return new Subject(name, subjectCode, studyUnit, studyUnitType,
                           studyType, semester, mustStudySubject, parallelSubject, description, rawSoup, courseId);
            }
            return null;
        }
    }
}
