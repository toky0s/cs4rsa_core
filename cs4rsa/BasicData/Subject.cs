using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using cs4rsa.Crawler;
using cs4rsa.Helpers;
using cs4rsa.Implements;
using HtmlAgilityPack;

namespace cs4rsa.BasicData
{
    /// <summary>
    /// Class này chứa toàn bộ thông tin về một môn học.
    /// Bao gồm danh sách tất cả các ClassGroup có trong môn học đó.
    /// </summary>
    public class Subject
    {
        private readonly string _subjectName;
        private readonly string _subjectCode;
        private readonly string _studyUnit;
        private readonly string _studyUnitType;
        private readonly string _studyType;
        private readonly string _semester;
        private readonly string[] _mustStudySubject;
        private readonly string[] _parallelSubject;
        private readonly string _description;
        private readonly string _rawSoup;
        private readonly string _courseId;

        private List<string> _tempTeachers = new List<string>();
        public List<string> TempTeachers => _tempTeachers;

        private List<Teacher> _teachers = new List<Teacher>();
        public List<Teacher> Teachers => _teachers;
        private List<ClassGroup> classGroups = new List<ClassGroup>();
        public List<ClassGroup> ClassGroups => classGroups;

        public string Name => _subjectName;
        public string SubjectCode => _subjectCode;
        public int StudyUnit => int.Parse(_studyUnit);
        public string[] MustStudySubject => _mustStudySubject;
        public string[] ParallelSubject => _parallelSubject;
        public string Desciption => _description;
        public string RawSoup => _rawSoup;
        public string CourseId => _courseId;

        public Subject(string name, string subjectCode, string studyUnit,
                        string studyUnitType, string studyType, string semester, 
                        string mustStudySubject, string parallelSubject,
                        string description, string rawSoup, string courseId)
        {
            _subjectName = name;
            _subjectCode = subjectCode;
            _studyUnit = studyUnit;
            _studyUnitType = studyUnitType;
            _studyType = studyType;
            _semester = semester;
            _mustStudySubject = SubjectSpliter(mustStudySubject);
            _parallelSubject = SubjectSpliter(parallelSubject);
            _description = description;
            _rawSoup = rawSoup;
            _courseId = courseId;
            GetClassGroups();
        }

        private string[] GetClassGroupNames()
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
        private void GetClassGroups()
        {
            if (classGroups.Count() == 0)
            {
                List<SchoolClass> schoolClasses = GetSchoolClasses();
                foreach (string classGroupName in GetClassGroupNames())
                {
                    ClassGroup classGroup = new ClassGroup(classGroupName, _subjectCode);

                    string pattern = string.Format(@"^({0})[0-9]*$", classGroupName);
                    Regex regexName = new Regex(pattern);
                    for (int i = 0; i < schoolClasses.Count(); i++)
                    {
                        if (regexName.IsMatch(schoolClasses[i].SchoolClassName))
                        {
                            schoolClasses[i].ClassGroupName = classGroupName;
                            classGroup.AddSchoolClass(schoolClasses[i]);
                        }
                    }
                    classGroups.Add(classGroup);
                }
            }
        }

        private List<SchoolClass> GetSchoolClasses()
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
        private SchoolClass GetSchoolClass(HtmlNode trTagClassLop)
        {
            HtmlNode[] tdTags = trTagClassLop.SelectNodes("td").ToArray();
            HtmlNode aTag = tdTags[0].SelectSingleNode("a");

            string urlToSubjectDetailPage = GetSubjectDetailPageURL(aTag);
            //teacher parser
            string teacherName = GetTeacherName(trTagClassLop);
            Teacher teacher = GetTeacherFromURL(urlToSubjectDetailPage);

            List<string> tempTeachers = new List<string>();
            tempTeachers.Add(teacherName);
            List<Teacher> teachers = new List<Teacher>();
            teachers.Add(teacher);
            //teacher parser

            //meta data
            DayPlaceMetaData metaData = new MetaDataCrawler(urlToSubjectDetailPage).ToDayPlaceMetaData();
            //meta data

            string schoolClassName = aTag.InnerText.Trim();
            string registerCode = tdTags[1].SelectSingleNode("a").InnerText.Trim();
            string studyType = tdTags[2].InnerText.Trim();
            string emptySeat = tdTags[3].InnerText.Trim();

            // Hạn bắt đầu và kết thúc đăng ký (đôi lúc nó sẽ không có nên mình sẽ check null đoạn này)
            string registrationTermStart;
            string registrationTermEnd;
            string[] registrationTerm = StringHelper.SplitAndRemoveAllSpace(tdTags[4].InnerText);
            if (registrationTerm.Count() == 0)
            {
                registrationTermStart = null;
                registrationTermEnd = null;
            }
            else
            {
                registrationTermStart = registrationTerm[0];
                registrationTermEnd = registrationTerm[1];
            }

            string studyWeekString = tdTags[5].InnerText.Trim();
            StudyWeek studyWeek = new StudyWeek(studyWeekString);

            Schedule schedule = new ScheduleParser(tdTags[6]).ToSchedule();

            string[] rooms = StringHelper.SplitAndRemoveAllSpace(tdTags[7].InnerText).Distinct().ToArray();

            Regex regexSpace = new Regex(@"^ *$");
            string[] locations = StringHelper.SplitAndRemoveNewLine(tdTags[8].InnerText);
            // remove space in locations
            locations = locations.Where(item => regexSpace.IsMatch(item) == false).ToArray();
            locations = locations.Select(item => item.Trim()).Distinct().ToArray();
            List<Place> places = new List<Place>();
            places = locations.Select(item => BasicDataConverter.ToPlace(item)).ToList();

            string registrationStatus = tdTags[10].InnerText.Trim();
            string implementationStatus = tdTags[11].InnerText.Trim();

            SchoolClass schoolClass = new SchoolClass(schoolClassName, registerCode, studyType,
                                        emptySeat, registrationTermEnd, registrationTermStart, studyWeek, schedule,
                                        rooms, places, teachers, tempTeachers, registrationStatus, implementationStatus, urlToSubjectDetailPage, metaData);
            return schoolClass;
        }

        /// <summary>
        /// Trả về teacher Name của một school class. Đồng thời thêm teacher Name này vào
        /// temp teachers nhằm đảm bảo không thất thoát các teacher không có detail page.
        /// Cải thiện độ chính xác của bộ lọc teacher.
        /// </summary>
        /// <param name="trTagClassLop"></param>
        /// <returns></returns>
        private string GetTeacherName(HtmlNode trTagClassLop)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(trTagClassLop.InnerHtml);
            HtmlNode teacherTdNode = doc.DocumentNode.SelectSingleNode("//td[10]");
            string[] slices = StringHelper.SplitAndRemoveAllSpace(teacherTdNode.InnerText);
            string teacherName = String.Join(" ", slices);
            if (teacherName != "")
            {
            _tempTeachers.Add(teacherName);
            _tempTeachers = _tempTeachers.Distinct().ToList();
            }
            return teacherName;
        }

        private HtmlNode[] GetTrTagsWithClassLop()
        {
            HtmlNode[] trTags = GetListTrTagInCalendar();
            HtmlNode[] trTagsWithClassLop = trTags
                .Where(node => node.SelectSingleNode("td").Attributes["class"].Value == "hit").ToArray();
            return trTagsWithClassLop;
        }

        private HtmlNode[] GetListTrTagInCalendar()
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(_rawSoup);
            HtmlNode tableTbCalendar = htmlDocument.DocumentNode.Descendants("table").ToArray()[3];
            HtmlNode bodyCalendar = tableTbCalendar.Descendants("tbody").ToArray()[0];
            HtmlNode[] trTags = bodyCalendar.Descendants("tr").ToArray();
            return trTags;
        }

        private string GetTeacherInfoPageURL(string urlSubjectDetailPage)
        {
            HtmlWeb htmlWeb = new HtmlWeb();
            HtmlDocument htmlDocument = htmlWeb.Load(urlSubjectDetailPage);
            HtmlNode aTag = htmlDocument.DocumentNode.SelectSingleNode(@"//td[contains(@class, 'no-leftborder')]/a");
            if (aTag == null)
            {
                return null;
            }
            return "http://courses.duytan.edu.vn/Sites/" + aTag.Attributes["href"].Value;
        }

        private string GetSubjectDetailPageURL(HtmlNode aTag)
        {
            return "http://courses.duytan.edu.vn/Sites/" + aTag.Attributes["href"].Value;
        }

        /// <summary>
        /// Nạp Teacher mới (nếu chưa có) vào Subject này thông qua url đồng thời trả về một teacher vừa mới được parse.
        /// </summary>
        /// <param name="url">Chuỗi url tới trang chi tiết nhóm lớp.</param>
        /// <returns></returns>
        private Teacher GetTeacherFromURL(string url)
        {
            string teacherDetailPageURL = GetTeacherInfoPageURL(url);
            TeacherCrawler teacherCrawler = new TeacherCrawler(teacherDetailPageURL)
            {
                TeacherSaver = new TeacherSaver()
            };
            Teacher teacher = teacherCrawler.ToTeacher();

            if (teacher != null && !_teachers.Contains(teacher))
                _teachers.Add(teacher);

            return teacher;
        }

        /// <summary>
        /// Tách mã môn từ một chuỗi, nếu không phát hiện nó trả về null.
        /// </summary>
        /// <returns>Mã môn (ví dụ CS 414)</returns>
        private string[] SubjectSpliter(string text)
        {
            if (text.Equals("(Không có Môn học Tiên quyết)") ||
                text.Equals("(Không có Môn học Song hành)"))
                return null;
            Regex regex = new Regex(@"(?<=\()(.*?)(?=\))");
            MatchCollection matchSubject = regex.Matches(text);
            string[] subjects = new string[matchSubject.Count];
            for(int i=0; i<matchSubject.Count; ++i)
                subjects[i] = matchSubject[i].Value;
            return subjects;
        }
    }
}
