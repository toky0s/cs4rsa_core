using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.BasicData;
using HtmlAgilityPack;
using cs4rsa.Crawler;


namespace cs4rsa.Crawler
{
    public class SubjectCrawler
    {

        private HomeCourseSearch homeCourseSearch = new HomeCourseSearch();
        private string subjectCode;
        private string SubjectCode
        {
            get { return subjectCode; }
        }

        /// <summary>
        /// Get a Subject of DTU.
        /// </summary>
        /// <param name="discipline">Hai chữ cái đầu của mã môn (CS).</param>
        /// <param name="keyword1">Các chữ số đằng sau (414).</param>
        public SubjectCrawler(string discipline, string keyword1)
        {
            this.subjectCode = discipline + " " + keyword1;
        }

        /// <summary>
        /// Nhận vào một mã môn học.
        /// </summary>
        /// <param name="subjectCode">Mã môn học, ví dụ CS414</param>
        public SubjectCrawler(string subjectCode)
        {
            this.subjectCode = subjectCode;
        }


        public Subject ToSubject()
        {
            string courseId = HomeCourseSearch.GetCourseId(this.subjectCode);
            string semesterId = homeCourseSearch.CurrentSemesterValue;

            string url = String.Format("http://courses.duytan.edu.vn/Modules/academicprogram/CourseClassResult.aspx?courseid={0}&semesterid={1}&timespan=71", courseId, semesterId);
            HtmlWeb htmlWeb = new HtmlWeb();
            HtmlDocument htmlDocument = htmlWeb.Load(url);
            HtmlNode table = htmlDocument.DocumentNode.Descendants("table").ToArray()[2];
            HtmlNode[] trTags = table.Descendants("tr").ToArray();
            string subjectCode = trTags[0].Elements("td").ToArray()[1].InnerText.Trim();
            string name = HomeCourseSearch.GetSubjectName(subjectCode);

            string studyUnit = trTags[1].Elements("td").ToArray()[1].GetDirectInnerText().Split(' ')[24];
            string studyUnitType = trTags[2].Elements("td").ToArray()[1].InnerText.Trim();
            string studyType = trTags[3].Elements("td").ToArray()[1].InnerText.Trim();
            string semester = trTags[4].Elements("td").ToArray()[1].InnerText.Trim();

            string mustStudySubject = trTags[5].Elements("td").ToArray()[1].InnerText.Trim();
            string parallelSubject = trTags[6].Elements("td").ToArray()[1].InnerText.Trim();

            string description = trTags[7].Elements("td").ToArray()[1].InnerText.Trim();

            string rawSoup = htmlDocument.DocumentNode.OuterHtml;
            return new Subject(name, subjectCode, studyUnit, studyUnitType,
                       studyType, semester, mustStudySubject, parallelSubject, description, rawSoup);
        }
    }
}
