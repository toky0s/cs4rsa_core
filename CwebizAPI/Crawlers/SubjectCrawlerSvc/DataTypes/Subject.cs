﻿using System.Text.RegularExpressions;
using CwebizAPI.Crawlers.SubjectCrawlerSvc.DataTypes.Enums;
using CwebizAPI.Crawlers.SubjectCrawlerSvc.Utils;
using CwebizAPI.Crawlers.TeacherCrawlerSvc.Crawlers.Interfaces;
using CwebizAPI.Crawlers.TeacherCrawlerSvc.Models;
using CwebizAPI.Db.Interfaces;
using CwebizAPI.Utils;
using HtmlAgilityPack;

namespace CwebizAPI.Crawlers.SubjectCrawlerSvc.DataTypes
{
    public class Subject
    {
        private readonly HtmlWeb _htmlWeb;

        private readonly string _studyUnit;
        private readonly string _studyUnitType;
        private readonly string _studyType;
        private readonly string _semester;
        private readonly string _rawSoup;
        
        public List<string> TempTeachers { get; set; }
        public List<ClassGroup> ClassGroups { get; }

        public string Name { get; }
        public string SubjectCode { get; }
        public int StudyUnit => int.Parse(_studyUnit);
        public string StudyUnitType => _studyUnitType;
        public string StudyType => _studyType;
        public string Semester => _semester;
        public IEnumerable<string> MustStudySubject { get; }
        public IEnumerable<string> ParallelSubject { get; }
        public string Desciption { get; }
        public readonly string CourseId;

        private Subject(string name, string subjectCode, string studyUnit,
                        string studyUnitType, string studyType, string semester,
                        string mustStudySubject, string parallelSubject,
                        string description, string rawSoup, string courseId,
                        HtmlWeb htmlWeb)
        {
            _htmlWeb = htmlWeb;

            _studyUnit = studyUnit;
            _studyUnitType = studyUnitType;
            _studyType = studyType;
            _semester = semester;
            _rawSoup = rawSoup;
            
            TempTeachers = new List<string>();
            ClassGroups = new List<ClassGroup>();

            Name = name;
            SubjectCode = subjectCode;
            MustStudySubject = SubjectSpliter(mustStudySubject);
            ParallelSubject = SubjectSpliter(parallelSubject);
            Desciption = description;
            CourseId = courseId;
        }

        /// <summary>
        /// Một môn được xem là Special Subject khi chúng có nhiều hơn 1 mã đăng ký
        /// trong một Class Group. Các môn như CHE 101 (Hoá đại cương), BIO 101 (Sinh học đại cương)
        /// được xem là một Special Subject.
        /// </summary>
        /// <returns></returns>
        public bool IsSpecialSubject()
        {
            bool isSpecialSubject = false;
            foreach (ClassGroup classGroup in ClassGroups)
            {
                int registerCodeCount = 0;
                foreach (SchoolClass schoolClass in classGroup.SchoolClasses)
                {
                    string registerCode = schoolClass.RegisterCode;
                    if (registerCode != string.Empty)
                    {
                        registerCodeCount++;
                    }
                }
                if (registerCodeCount > 1)
                {
                    isSpecialSubject = true;
                    break;
                }
            }
            return isSpecialSubject;
        }

        private IEnumerable<string> GetClassGroupNames()
        {
            IEnumerable<HtmlNode> trTags = GetListTrTagInCalendar();
            IEnumerable<HtmlNode> classGroupTrTags = trTags
                                        .Where(node => node.SelectSingleNode("td").Attributes["class"].Value == "nhom-lop");
            IEnumerable<string> classGroupNames = classGroupTrTags.Select(node => node.InnerText.Trim());
            return classGroupNames;
        }

        /// <summary>
        /// Lấy ra các ClassGroup và thêm chúng vào danh sách chứa
        /// ClassGroup của đối tượng này bằng cách match các tên 
        /// với các SchoolClass nó tên giống.
        /// </summary>
        public async Task GetClassGroups()
        {
            if (ClassGroups.Count == 0)
            {
                IEnumerable<SchoolClass> schoolClasses = await GetSchoolClasses();
                foreach (string classGroupName in GetClassGroupNames())
                {
                    string pattern = $@"^({classGroupName})[0-9]*$";
                    Regex regexName = new(pattern);
                    IEnumerable<SchoolClass> schoolClassesByName = schoolClasses.Where(sc => regexName.IsMatch(sc.SchoolClassName));
                    foreach (SchoolClass schoolClass in schoolClassesByName)
                    {
                        schoolClass.ClassGroupName = classGroupName;
                    }

                    ClassGroup classGroup = new(classGroupName, SubjectCode, Name);
                    classGroup.AddRangeSchoolClass(schoolClassesByName);
                    ClassGroups.Add(classGroup);
                }
            }
        }

        private async Task<IEnumerable<SchoolClass>> GetSchoolClasses()
        {
            List<SchoolClass> schoolClasses = new();
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
        /// <param name="trTagClassLop">Thẻ tr có class="lop"</param>
        /// <returns>
        /// SchoolClass - Lớp thành phần là con của một ClassGroup, 
        /// sẽ trả về null nếu ClassGroup đấy không chứa bất cứ SchoolClass nào.
        /// 
        /// Phát hiện lúc đêm  04/11/2022, TOU 151 Tổng quan du lịch, 
        /// ClassGroup TOU 151 Q không chứa bất cứ SchoolClass nào.
        /// 
        /// <see href="https://github.com/toky0s/cs4rsa_core/issues/79"/> 
        /// </returns>
        private async Task<SchoolClass> GetSchoolClass(HtmlNode trTagClassLop)
        {
            if (!trTagClassLop.HasChildNodes)
            {
                return null;
            }

            HtmlNodeCollection tdTags = trTagClassLop.SelectNodes("td");
            HtmlNode aTag = tdTags[0].SelectSingleNode("a");

            string urlToSubjectDetailPage = GetSubjectDetailPageURL(aTag);

            #region Teacher Parser
            /* 
             * ACC 448 - Thực Tập Tốt Nghiệp, cái môn củ chuối này nó không có
             * tên giảng viên (tên giảng viên bằng Rỗng), dẫn đến nó cái Dialog
             * tìm kiếm nó chạy mãi không dừng. Và tui sẽ fix nó hôm nay.
             * Ngày Mùng 5 Tết 2022 
             * 
             * Trong tình trạng mạng yếu, việc cào thêm dữ liệu của giảng viên là
             * không ưu tiên, hãy set cờ withTeacher về false. Hai list teacher
             * và tmpTeacher sẽ về rỗng.
             * 
             * Thông tin của Teacher sẽ ưu tiên lấy từ DB ra. Các môn đã có cache
             * hầu hết sẽ có thông tin giảng viên đi kèm.
             * 
             * Created Date:
             *  XinTA - Ngày 19/1/2023
             *  
             * Updated Date:
             *  XinTA - Ngày 29/01/2023 - Cập nhật tài liệu
             *  XinTA - Ngày 07/03/2023 - Add Debug, update if clause flow.
             *  XinTA - Ngày 19/07/2023 - Project CWEBIZ - Integrate
             */
            List<string> tempTeachers = new();
            string teacherName = GetTeacherName(trTagClassLop);
            // 1. Add tmp teachers
            if (!string.IsNullOrEmpty(teacherName))
            {
                tempTeachers.Add(teacherName);
            }
            #endregion

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
            StudyWeek studyWeek = new(studyWeekString);

            Schedule schedule = new ScheduleParser(tdTags[6]).ToSchedule();

            IEnumerable<string> rooms = StringHelper
                .SplitAndRemoveAllSpace(tdTags[7].InnerText)
                .Distinct();

            Regex regexSpace = new(@"^ *$");
            IEnumerable<string> locations = StringHelper
                .SplitAndRemoveNewLine(tdTags[8].InnerText)
                .Where(item => regexSpace.IsMatch(item) == false);

            IEnumerable<Place> places = locations
                .Distinct()
                .Select(item => BasicDataConverter.ToPlace(item));

            #region MetaData
            // Mỗi SchoolClass đều có một MetaData map giữa Thứ-Giờ-Phòng-Nơi học.
            List<DayOfWeek> dayOfWeeks = schedule.GetSchoolDays().ToList();
            int metaCount = dayOfWeeks.Count;
            IEnumerable<string> roomsText = StringHelper.SplitAndRemoveAllSpace(tdTags[7].InnerText);
            // Lúc này Room được set Name và chưa được set Place.
            List<Room> roomsForMetaData = roomsText.Select(item => new Room(item)).ToList();
            IEnumerable<string> locationsForMetaData = locations.Select(item => item.Trim());
            List<Place> placesForMetaData = locationsForMetaData.Select(item => BasicDataConverter.ToPlace(item)).ToList();

            DayPlaceMetaData metaData = new();
            for (int i = 0; i < metaCount; i++)
            {
                // Set Place cho Room ở đây.
                roomsForMetaData[i].Place = placesForMetaData[i];
                DayRoomPlace dayPlacePair = new(dayOfWeeks[i], roomsForMetaData[i], placesForMetaData[i]);
                metaData.AddDayTimePair(dayOfWeeks[i], dayPlacePair);
            }
            #endregion

            string registrationStatus = tdTags[10].InnerText.Trim();
            string implementationStatus = tdTags[11].InnerText.Trim();

            SchoolClass schoolClass = new(schoolClassName, registerCode, studyType, emptySeat,
                                        registrationTermEnd, registrationTermStart, studyWeek, schedule,
                                        rooms, places, tempTeachers,
                                        registrationStatus, implementationStatus,
                                        urlToSubjectDetailPage, metaData, SubjectCode);
            return schoolClass;
        }

        /// <summary>
        /// Trả về teacherModel Name của một school class. Đồng thời thêm teacherModel Name này vào
        /// temp teachers nhằm đảm bảo không thất thoát các teacherModel không có detail page.
        /// Cải thiện độ chính xác của bộ lọc teacherModel.
        /// </summary>
        /// <param name="trTagClassLop">HtmlNode với giá trị class="lop".</param>
        /// <returns>Tên giảng viên</returns>
        private string GetTeacherName(HtmlNode trTagClassLop)
        {
            HtmlDocument doc = new();
            doc.LoadHtml(trTagClassLop.InnerHtml);
            HtmlNode teacherTdNode = doc.DocumentNode.SelectSingleNode("//td[10]");
            string[] slices = teacherTdNode.InnerText.SplitAndRemoveAllSpace();
            string teacherName = string.Join(" ", slices);
            if (string.IsNullOrWhiteSpace(teacherName)) return teacherName;
            TempTeachers.Add(teacherName);
            TempTeachers = TempTeachers.Distinct().ToList();
            return teacherName;
        }

        private IEnumerable<HtmlNode> GetTrTagsWithClassLop()
        {
            IEnumerable<HtmlNode> trTags = GetListTrTagInCalendar();
            return trTags.Where(node => node.SelectSingleNode("td").Attributes["class"].Value == "hit");
        }

        private IEnumerable<HtmlNode> GetListTrTagInCalendar()
        {
            HtmlDocument htmlDocument = new();
            htmlDocument.LoadHtml(_rawSoup);
            HtmlNode tableTbCalendar = htmlDocument.DocumentNode.Descendants("table").ToArray()[3];
            HtmlNode bodyCalendar = tableTbCalendar.Descendants("tbody").ToArray()[0];
            IEnumerable<HtmlNode> trTags = bodyCalendar.Descendants("tr");
            return trTags;
        }

        private async Task<string> GetTeacherInfoPageURL(string urlSubjectDetailPage)
        {
            HtmlDocument htmlDocument = await _htmlWeb.LoadFromWebAsync(urlSubjectDetailPage);
            HtmlNode aTag = htmlDocument.DocumentNode.SelectSingleNode(@"//td[contains(@class, 'no-leftborder')]/a");
            return aTag == null ? null : "http://courses.duytan.edu.vn/Sites/" + aTag.Attributes["href"].Value;
        }

        private static string GetSubjectDetailPageURL(HtmlNode aTag)
        {
            return "http://courses.duytan.edu.vn/Sites/" + aTag.Attributes["href"].Value;
        }

        /// <summary>
        /// Tách mã môn từ một chuỗi.
        /// </summary>
        /// <returns>Mã môn (ví dụ CS 414)</returns>
        private static IEnumerable<string> SubjectSpliter(string text)
        {
            if (text.Equals("(Không có Môn học Tiên quyết)") ||
                text.Equals("(Không có Môn học Song hành)", StringComparison.Ordinal))
            {
                yield break;
            }

            Regex regex = new(@"(?<=\()(.*?)(?=\))");
            MatchCollection matchSubject = regex.Matches(text);
            for (int i = 0; i < matchSubject.Count; ++i)
            {
                yield return matchSubject[i].Value;
            }
        }

        private async Task<Subject> InitializeAsync()
        {
            await GetClassGroups();
            return this;
        }

        /// <summary>
        /// Khởi tạo một Subject thông qua một Async Factory method.
        /// </summary>
        /// <param name="name">Tên môn học</param>
        /// <param name="subjectCode">Mã môn</param>
        /// <param name="studyUnit">Số đơn vị học tập</param>
        /// <param name="studyUnitType">Loại đơn vị học tập</param>
        /// <param name="studyType"></param>
        /// <param name="semester">Học kỳ</param>
        /// <param name="mustStudySubject">Các môn tiên quyết</param>
        /// <param name="parallelSubject">Các môn song hành</param>
        /// <param name="description">Mô tả môn học</param>
        /// <param name="rawSoup">Chuỗi phân tích HTML gốc</param>
        /// <param name="courseId">Course ID</param>
        /// <returns>Task Subject</returns>
        public static Task<Subject> CreateAsync(
            string name, string subjectCode, string studyUnit,
            string studyUnitType, string studyType, string semester,
            string mustStudySubject, string parallelSubject,
            string description, string rawSoup, string courseId,
            HtmlWeb htmlWeb)
        {
            Subject ret = new(name, subjectCode, studyUnit, studyUnitType, studyType,
                semester, mustStudySubject, parallelSubject, description,
                rawSoup, courseId, htmlWeb);
            return ret.InitializeAsync();
        }
    }
}
