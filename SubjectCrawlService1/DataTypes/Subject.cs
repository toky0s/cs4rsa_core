using Cs4rsaDatabaseService.Models;
using Cs4rsaDatabaseService.DataProviders;
using HelperService;
using HtmlAgilityPack;
using SubjectCrawlService1.DataTypes;
using SubjectCrawlService1.DataTypes.Enums;
using SubjectCrawlService1.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TeacherCrawlerService1.Crawlers.Interfaces;
using TeacherCrawlerService1.Crawlers;
using System.Threading.Tasks;
using Cs4rsaDatabaseService.Interfaces;

namespace SubjectCrawlService1.DataTypes
{
    public class Subject
    {
        private ITeacherCrawler _teacherCrawler;
        private IUnitOfWork _unitOfWork;
        private readonly string _studyUnit;
        private readonly string _studyUnitType;
        private readonly string _studyType;
        private readonly string _semester;
        private List<string> _tempTeachers = new();
        public List<string> TempTeachers => _tempTeachers;
        private List<Teacher> _teachers = new();
        public List<Teacher> Teachers => _teachers;
        private List<ClassGroup> classGroups = new();
        public List<ClassGroup> ClassGroups => classGroups;

        public string Name { get; }
        public string SubjectCode { get; }
        public int StudyUnit => int.Parse(_studyUnit);
        public string[] MustStudySubject { get; }
        public string[] ParallelSubject { get; }
        public string Desciption { get; }
        public string RawSoup { get; }
        public int CourseId { get; }

        private Subject(string name, string subjectCode, string studyUnit,
                        string studyUnitType, string studyType, string semester, 
                        string mustStudySubject, string parallelSubject,
                        string description, string rawSoup, int courseId, 
                        ITeacherCrawler teacherCrawler, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _teacherCrawler = teacherCrawler;
            Name = name;
            SubjectCode = subjectCode;
            _studyUnit = studyUnit;
            _studyUnitType = studyUnitType;
            _studyType = studyType;
            _semester = semester;
            MustStudySubject = SubjectSpliter(mustStudySubject);
            ParallelSubject = SubjectSpliter(parallelSubject);
            Desciption = description;
            RawSoup = rawSoup;
            CourseId = courseId;
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
        public async Task GetClassGroups()
        {
            if (classGroups.Count == 0)
            {
                List<SchoolClass> schoolClasses = await GetSchoolClasses();
                foreach (string classGroupName in GetClassGroupNames())
                {
                    ClassGroup classGroup = new(classGroupName, SubjectCode);

                    string pattern = $@"^({classGroupName})[0-9]*$";
                    Regex regexName = new(pattern);
                    for (int i = 0; i < schoolClasses.Count; i++)
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

        private async Task<List<SchoolClass>> GetSchoolClasses()
        {
            List<SchoolClass> schoolClasses = new List<SchoolClass>();
            foreach (HtmlNode trTag in GetTrTagsWithClassLop())
            {
                SchoolClass schoolClass = await GetSchoolClass(trTag);
                schoolClasses.Add(schoolClass);
            }
            return schoolClasses;
        }

        /// <summary>
        /// Trả về một SchoolClass dựa theo tr tag có class="lop" được truyền vào phương thức này.
        /// </summary>
        /// <param name="trTagClassLop">Thẻ tr có class="lop".</param>
        /// <returns></returns>
        private async Task<SchoolClass> GetSchoolClass(HtmlNode trTagClassLop)
        {
            HtmlNode[] tdTags = trTagClassLop.SelectNodes("td").ToArray();
            HtmlNode aTag = tdTags[0].SelectSingleNode("a");

            string urlToSubjectDetailPage = GetSubjectDetailPageURL(aTag);
            //teacher parser
            string teacherName = GetTeacherName(trTagClassLop);
            Teacher teacher = await GetTeacherFromURL(urlToSubjectDetailPage);

            List<string> tempTeachers = new List<string>();
            tempTeachers.Add(teacherName);
            List<Teacher> teachers = new List<Teacher>();
            teachers.Add(teacher);
            //teacher parser

            string schoolClassName = aTag.InnerText.Trim();
            string registerCode = tdTags[1].SelectSingleNode("a").InnerText.Trim();
            string studyType = tdTags[2].InnerText.Trim();
            string emptySeat = tdTags[3].InnerText.Trim();

            // Hạn bắt đầu và kết thúc đăng ký (đôi lúc nó sẽ không có nên mình sẽ check null đoạn này)
            string registrationTermStart;
            string registrationTermEnd;
            string[] registrationTerm = StringHelper.SplitAndRemoveAllSpace(tdTags[4].InnerText);
            if (registrationTerm.Length == 0)
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

            Regex regexSpace = new(@"^ *$");
            List<string> locations = StringHelper.SplitAndRemoveNewLine(tdTags[8].InnerText).ToList();
            // remove space in locations
            locations = locations.Where(item => regexSpace.IsMatch(item) == false).ToList();
            
            List<string> locationsForPlace = locations.Select(item => item.Trim()).Distinct().ToList();
            List<Place> places = new List<Place>();
            places = locationsForPlace.Select(item => BasicDataConverter.ToPlace(item)).ToList();

            #region MetaData
            // Mỗi SchoolClass đều có một MetaData map giữa Thứ-Giờ-Phòng-Nơi học.
            List<DayOfWeek> dayOfWeeks = schedule.GetSchoolDays();
            int metaCount = dayOfWeeks.Count;
            List<string> roomsText = StringHelper.SplitAndRemoveAllSpace(tdTags[7].InnerText).ToList();
            List<Room> roomsForMetaData = roomsText.Select(item => new Room(item)).ToList();
            List<string> locationsForMetaData = locations.Select(item => item.Trim()).ToList();
            List<Place> placesForMetaData = locationsForMetaData.Select(item => BasicDataConverter.ToPlace(item)).ToList();

            DayPlaceMetaData metaData = new DayPlaceMetaData();
            for (int i = 0; i < metaCount; i++)
            {
                DayPlacePair dayPlacePair = new DayPlacePair(dayOfWeeks[i], roomsForMetaData[i], placesForMetaData[i]);
                metaData.AddDayTimePair(dayOfWeeks[i], dayPlacePair);
            }
            #endregion

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
            HtmlDocument doc = new();
            doc.LoadHtml(trTagClassLop.InnerHtml);
            HtmlNode teacherTdNode = doc.DocumentNode.SelectSingleNode("//td[10]");
            string[] slices = StringHelper.SplitAndRemoveAllSpace(teacherTdNode.InnerText);
            string teacherName = string.Join(" ", slices);
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
            htmlDocument.LoadHtml(RawSoup);
            HtmlNode tableTbCalendar = htmlDocument.DocumentNode.Descendants("table").ToArray()[3];
            HtmlNode bodyCalendar = tableTbCalendar.Descendants("tbody").ToArray()[0];
            HtmlNode[] trTags = bodyCalendar.Descendants("tr").ToArray();
            return trTags;
        }

        private static async Task<string> GetTeacherInfoPageURL(string urlSubjectDetailPage)
        {
            HtmlWeb htmlWeb = new();
            HtmlDocument htmlDocument = await htmlWeb.LoadFromWebAsync(urlSubjectDetailPage);
            HtmlNode aTag = htmlDocument.DocumentNode.SelectSingleNode(@"//td[contains(@class, 'no-leftborder')]/a");
            return aTag == null ? null : "http://courses.duytan.edu.vn/Sites/" + aTag.Attributes["href"].Value;
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
        private async Task<Teacher> GetTeacherFromURL(string url)
        {
            string teacherDetailPageURL = await GetTeacherInfoPageURL(url);
            TeacherCrawler teacherCrawler = new(_unitOfWork);
            Teacher teacher = await teacherCrawler.Crawl(teacherDetailPageURL);

            if (teacher != null && !_teachers.Contains(teacher))
            {
                _teachers.Add(teacher);
            }

            return teacher;
        }

        /// <summary>
        /// Tách mã môn từ một chuỗi, nếu không phát hiện nó trả về null.
        /// </summary>
        /// <returns>Mã môn (ví dụ CS 414)</returns>
        private static string[] SubjectSpliter(string text)
        {
            if (text.Equals("(Không có Môn học Tiên quyết)") ||
                text.Equals("(Không có Môn học Song hành)", StringComparison.Ordinal))
            {
                return null;
            }

            Regex regex = new Regex(@"(?<=\()(.*?)(?=\))");
            MatchCollection matchSubject = regex.Matches(text);
            string[] subjects = new string[matchSubject.Count];
            for (int i = 0; i < matchSubject.Count; ++i)
            {
                subjects[i] = matchSubject[i].Value;
            }

            return subjects;
        }

        private async Task<Subject> InitializeAsync()
        {
            await GetClassGroups();
            return this;
        }

        public static Task<Subject> CreateAsync(string name, string subjectCode, string studyUnit,
                        string studyUnitType, string studyType, string semester,
                        string mustStudySubject, string parallelSubject,
                        string description, string rawSoup, int courseId,
                        ITeacherCrawler teacherCrawler, IUnitOfWork unitOfWork)
        {
            Subject ret = new(name, subjectCode, studyUnit, studyUnitType, studyType, 
                semester, mustStudySubject, parallelSubject, description,
                rawSoup, courseId, teacherCrawler, unitOfWork);
            return ret.InitializeAsync();
        }
    }
}
