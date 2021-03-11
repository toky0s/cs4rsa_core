using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using cs4rsa.Helpers;
using cs4rsa.Crawler;


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

        public string[] GetClassGroupNames()
        {
            HtmlNode[] trTags = GetListTrTagInCalendar();
            HtmlNode[] classGroupTrTags = trTags
                                        .Where(node => node.SelectSingleNode("td").Attributes["class"].Value == "nhom-lop")
                                        .ToArray();
            string[] classGroupNames = classGroupTrTags.Select(node => node.InnerText.Trim()).ToArray();
            return classGroupNames;
        }


        /// <summary>
        /// Trả về danh sách các nhóm lớp.
        /// </summary>
        /// <returns>List các ClassGroup.</returns>
        public List<ClassGroup> GetClassGroups()
        {
            List<ClassGroup> classGroups = new List<ClassGroup>();
            List<SchoolClass> schoolClasses = GetSchoolClasses();
            foreach (string classGroupName in GetClassGroupNames())
            {
                ClassGroup classGroup = new ClassGroup(classGroupName, subjectCode);
                
                string pattern = String.Format(@"^({0})[0-9]*$", classGroupName);
                Regex regexName = new Regex(pattern);
                for(int i=0; i<schoolClasses.Count(); i++)
                {
                    if (regexName.IsMatch(schoolClasses[i].ClassGroupName))
                    {
                        classGroup.AddSchoolClass(schoolClasses[i]);
                    }
                }
                classGroups.Add(classGroup);
            }
            return classGroups;
        }

        public List<SchoolClass> GetSchoolClasses()
        {
            List<SchoolClass> schoolClasses = new List<SchoolClass>();
            foreach (HtmlNode trTag in GetTrTagsWithClassLop())
            {
                SchoolClass schoolClass = GetSchoolClass(trTag);
                schoolClasses.Add(schoolClass);
            }
            return schoolClasses;
        }

        /// <summary>
        /// Trả về một SchoolClass dựa theo tr tag có class="lop" được truyền vào phương thức này.
        /// </summary>
        /// <param name="trTagClassLop">Thẻ tr có class="lop".</param>
        /// <returns></returns>
        public SchoolClass GetSchoolClass(HtmlNode trTagClassLop)
        {
            HtmlNode[] tdTags = trTagClassLop.SelectNodes("td").ToArray();
            HtmlNode aTag = tdTags[0].SelectSingleNode("a");
            string classGroupName = aTag.InnerText.Trim();
            string registerCode = tdTags[1].SelectSingleNode("a").InnerText.Trim();
            string studyType = tdTags[2].InnerText.Trim();
            string emptySeat = tdTags[3].InnerText.Trim();

            string[] registrationTerm = StringHelper.SplitAndRemoveAllSpace(tdTags[4].InnerText);
            string registrationTermStart = registrationTerm[0];
            string registrationTermEnd = registrationTerm[1];

            string studyWeekString = tdTags[5].InnerText.Trim();
            StudyWeek studyWeek = new StudyWeek(studyWeekString);

            Schedule schedule = new ScheduleParser(tdTags[6]).ToSchedule();

            string[] rooms = StringHelper.SplitAndRemoveAllSpace(tdTags[7].InnerText).Distinct().ToArray();

            Regex regexSpace = new Regex(@"^ *$");
            string[] locations = StringHelper.SplitAndRemoveNewLine(tdTags[8].InnerText);
            // remove space in locations
            locations = locations.Where(item => regexSpace.IsMatch(item) == false).ToArray();
            locations = locations.Select(item => item.Trim()).Distinct().ToArray();

            string teacher = String.Join(" ",StringHelper.SplitAndRemoveAllSpace(tdTags[9].InnerHtml));
            string registrationStatus = tdTags[10].InnerText.Trim();
            string implementationStatus = tdTags[11].InnerText.Trim();

            SchoolClass schoolClass = new SchoolClass(subjectCode, classGroupName, name, registerCode, studyType,
                                        emptySeat, registrationTermEnd, registrationTermStart, studyWeek, schedule,
                                        rooms, locations, teacher, registrationStatus, implementationStatus);
            return schoolClass;
        }

        public HtmlNode[] GetTrTagsWithClassLop()
        {
            HtmlNode[] trTags = GetListTrTagInCalendar();
            HtmlNode[] trTagsWithClassLop = trTags
                .Where(node => node.SelectSingleNode("td").Attributes["class"].Value == "hit").ToArray();
            return trTagsWithClassLop;
        }

        public HtmlNode[] GetListTrTagInCalendar()
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(rawSoup);
            HtmlNode tableTbCalendar = htmlDocument.DocumentNode.Descendants("table").ToArray()[3];
            HtmlNode bodyCalendar = tableTbCalendar.Descendants("tbody").ToArray()[0];
            HtmlNode[] trTags = bodyCalendar.Descendants("tr").ToArray();
            return trTags;
        }
    }
}
