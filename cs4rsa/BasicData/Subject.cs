using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using HtmlAgilityPack;

namespace cs4rsa.BasicData
{
    /// <summary>
    /// Class này chứa toàn bộ thông tin về một môn học.
    /// Đồng thời cũng là bộ cào data để tìm được ClassGroup và SchoolClass.
    /// </summary>
    public class Subject
    {
        private string name;
        private string subjectCode;
        private string studyUnit;
        private string studyUnitType;
        private string studyType;
        private string semester;
        private string mustStudySubject;
        private string parallelSubject;
        private string description;

        public string Name { get { return name; } set { name = value; } }
        public string SubjectCode { get { return subjectCode; } set { subjectCode = value; } }

        private readonly string rawSoup;
        public string RawSoup
        {
            get { return rawSoup; }
        }

        public Subject(string name, string subjectCode, string studyUnit, 
            string studyUnitType, string studyType, string semester, string mustStudySubject, string parallelSubject,
            string description, string rawSoup)
        {
            this.name = name;
            this.subjectCode = subjectCode;
            this.studyUnit = studyUnit;
            this.studyUnitType = studyUnitType;
            this.studyType = studyType;
            this.semester = semester;
            this.mustStudySubject = mustStudySubject;
            this.parallelSubject = parallelSubject;
            this.description = description;

            this.rawSoup = rawSoup;
        }

        override
        public string ToString()
        {
            return String.Format("<Subject {0} {1}>", SubjectCode, Name);
        }

        public void getClassGroups()
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(rawSoup);

        }

        public string[] GetClassGroupNames()
        {
            HtmlNode[] trTags = GetListTrTagInCalendar();
            HtmlNode[] classGroupTrTags = trTags
                                        .Where(node => node.SelectSingleNode("td").Attributes["class"].Value == "nhom-lop")
                                        .ToArray();
            string[] classGroupNames = classGroupTrTags
                                        .Select(node => node.InnerText.Trim())
                                        .ToArray();
            return classGroupNames;
        }

        public HtmlNode[] GetListTrTagInCalendar()
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(rawSoup);
            HtmlNode tableTbCalendar = htmlDocument.DocumentNode.Descendants("table").ToArray()[3];
            Console.WriteLine(tableTbCalendar.InnerHtml);
            HtmlNode bodyCalendar = tableTbCalendar.Descendants("tbody").ToArray()[0];
            HtmlNode[] trTags = bodyCalendar.Descendants("tr").ToArray();
            return trTags;
        }

        public void ShowInfo()
        {
            Console.WriteLine(name);
            Console.WriteLine(subjectCode);
            Console.WriteLine(studyUnit);
            Console.WriteLine(studyUnitType);
            Console.WriteLine(studyType);
            Console.WriteLine(semester);
            Console.WriteLine(mustStudySubject);
            Console.WriteLine(parallelSubject);
            Console.WriteLine(description);
        }
    }
}
