using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Cs4rsa.Common;
using Cs4rsa.Service.SubjectCrawler.Utils;
using HtmlAgilityPack;

namespace Cs4rsa.Service.SubjectCrawler.DataTypes
{
    public class Subject
    {
        private readonly string _studyUnit;
        private readonly string _studyUnitType;
        private readonly string _studyType;
        private readonly string _semester;
        private readonly string _rawSoup;
        
        public List<string> TempTeachers { get; set; }
        public List<string> TeacherUrls { get; private set; }
        public List<ClassGroup> ClassGroups { get; }

        public string Name { get; }
        public string SubjectCode { get; }
        public int StudyUnit => int.Parse(_studyUnit);
        public string StudyUnitType => _studyUnitType;
        public string StudyType => _studyType;
        public string Semester => _semester;
        public IEnumerable<string> MustStudySubject { get; }
        public IEnumerable<string> ParallelSubject { get; }
        public string Description { get; }
        public readonly string CourseId;

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
        public Subject(string name, string subjectCode, string studyUnit,
                        string studyUnitType, string studyType, string semester,
                        string mustStudySubject, string parallelSubject,
                        string description, string rawSoup, string courseId)
        {
            _studyUnit = studyUnit;
            _studyUnitType = studyUnitType;
            _studyType = studyType;
            _semester = semester;
            _rawSoup = rawSoup;
            
            TempTeachers = new List<string>();
            ClassGroups = new List<ClassGroup>();
            TeacherUrls = new List<string>();

            Name = name;
            SubjectCode = subjectCode;
            MustStudySubject = SubjectSplit(mustStudySubject);
            ParallelSubject = SubjectSplit(parallelSubject);
            Description = description;
            CourseId = courseId;
            
            GetClassGroups();
        }

        /// <summary>
        /// Một môn được xem là Special Subject khi chúng có nhiều hơn 1 mã đăng ký
        /// trong một Class Group. Các môn như CHE 101 (Hoá đại cương), BIO 101 (Sinh học đại cương)
        /// được xem là một Special Subject.
        /// </summary>
        /// <returns></returns>
        public bool IsSpecialSubject()
        {
            return ClassGroups
                .Select(classGroup => classGroup
                    .SchoolClasses
                    .Select(schoolClass => schoolClass.RegisterCode)
                    .Count(registerCode => registerCode != string.Empty))
                .Any(registerCodeCount => registerCodeCount > 1);
        }

        private IEnumerable<string> GetClassGroupNames()
        {
            try
            {
                var trTags = GetListTrTagInCalendar();
                var classGroupTrTags = trTags
                    .Where(node => node.SelectSingleNode("td").Attributes["class"].Value == "nhom-lop");
                var classGroupNames = classGroupTrTags.Select(node => node.InnerText.Trim());
                return classGroupNames;
            }
            catch (IndexOutOfRangeException)
            {
                return Enumerable.Empty<string>();
            }
        }

        /// <summary>
        /// Lấy ra các ClassGroup và thêm chúng vào danh sách chứa
        /// ClassGroup của đối tượng này bằng cách match các tên 
        /// với các SchoolClass nó tên giống.
        /// </summary>
        private void GetClassGroups()
        {
            var schoolClasses = GetSchoolClasses().ToList();
            foreach (var classGroupName in GetClassGroupNames())
            {
                var pattern = $@"^({classGroupName})[0-9]*$";
                var regexName = new Regex(pattern);
                var schoolClassesByName = schoolClasses
                    .Where(sc => regexName.IsMatch(sc.SchoolClassName))
                    .Select(sc =>
                    {
                        sc.ClassGroupName = classGroupName;
                        return sc;
                    });

                var classGroup = new ClassGroup(classGroupName, SubjectCode, Name);
                classGroup.AddRangeSchoolClass(schoolClassesByName);
                ClassGroups.Add(classGroup);
            }
        }

        private IEnumerable<SchoolClass> GetSchoolClasses()
        {
            return GetTrTagsWithClassLop().Select(GetSchoolClass);
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
        private SchoolClass GetSchoolClass(HtmlNode trTagClassLop)
        {
            if (!trTagClassLop.HasChildNodes)
            {
                return null;
            }

            var tdTags = trTagClassLop.SelectNodes("td");
            var aTag = tdTags[0].SelectSingleNode("a");

            var urlToSubjectDetailPage = GetSubjectDetailPageUrl(aTag);

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
             *  XinTA - Ngày 16/12/2023 - Migrate
             */
            var tempTeachers = new List<string>();
            var teacherName = GetTeacherName(trTagClassLop);
            // 1. Add tmp teachers
            if (!string.IsNullOrEmpty(teacherName))
            {
                tempTeachers.Add(teacherName);
            }

            string teacherUrl = GetTeacherInfoPageUrl(urlToSubjectDetailPage);
            TeacherUrls.Add(teacherUrl);
            #endregion

            var schoolClassName = aTag.InnerText.Trim();
            var registerCode = tdTags[1].SelectSingleNode("a").InnerText.Trim();
            var studyType = tdTags[2].InnerText.Trim();
            var emptySeat = tdTags[3].InnerText.Trim();

            // Hạn bắt đầu và kết thúc đăng ký (đôi lúc nó sẽ không có nên mình sẽ check null đoạn này)
            string registrationTermStart;
            string registrationTermEnd;
            var registrationTerm = tdTags[4].InnerText.SplitAndRemoveAllSpace();
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

            var studyWeekString = tdTags[5].InnerText.Trim();
            var studyWeek = new StudyWeek(studyWeekString);

            var schedule = new ScheduleParser(tdTags[6]).ToSchedule();

            var rooms = tdTags[7].InnerText
                .SplitAndRemoveAllSpace()
                .Distinct();

            var regexSpace = new Regex(@"^ *$");
            var locations = StringHelper
                .SplitAndRemoveNewLine(tdTags[8].InnerText)
                .Where(item => regexSpace.IsMatch(item) == false)
                .ToList();

            var places = locations
                .Distinct()
                .Select(BasicDataConverter.ToPlace);

            #region MetaData
            // Mỗi SchoolClass đều có một MetaData map giữa Thứ-Giờ-Phòng-Nơi học.
            var dayOfWeeks = schedule.GetSchoolDays().ToList();
            var metaCount = dayOfWeeks.Count;
            IEnumerable<string> roomsText = StringHelper.SplitAndRemoveAllSpace(tdTags[7].InnerText);
            // Lúc này Room được set Name và chưa được set Place.
            var roomsForMetaData = roomsText.Select(item => new Room(item)).ToList();
            var locationsForMetaData = locations.Select(item => item.Trim());
            var placesForMetaData = locationsForMetaData.Select(item => BasicDataConverter.ToPlace(item)).ToList();

            var metaData = new DayPlaceMetaData();
            for (var i = 0; i < metaCount; i++)
            {
                // Set Place cho Room ở đây.
                roomsForMetaData[i].Place = placesForMetaData[i];
                var dayPlacePair = new DayRoomPlace(dayOfWeeks[i], roomsForMetaData[i], placesForMetaData[i]);
                metaData.AddDayTimePair(dayOfWeeks[i], dayPlacePair);
            }
            #endregion

            var registrationStatus = tdTags[10].InnerText.Trim();
            var implementationStatus = tdTags[11].InnerText.Trim();

            var schoolClass = new SchoolClass(schoolClassName, registerCode, studyType, emptySeat,
                                        registrationTermEnd, registrationTermStart, studyWeek, schedule,
                                        rooms, places, tempTeachers,
                                        registrationStatus, implementationStatus,
                                        urlToSubjectDetailPage, metaData, SubjectCode);
            return schoolClass;
        }

        private string GetTeacherInfoPageUrl(string urlSubjectDetailPage)
        {
            var htmlWeb = new HtmlWeb();
            var htmlDocument = htmlWeb.Load(urlSubjectDetailPage);
            var aTag = htmlDocument.DocumentNode.SelectSingleNode(@"//td[contains(@class, 'no-leftborder')]/a");
            return aTag == null ? null : "http://courses.duytan.edu.vn/Sites/" + aTag.Attributes["href"].Value;
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
            var doc = new HtmlDocument();
            doc.LoadHtml(trTagClassLop.InnerHtml);
            var teacherTdNode = doc.DocumentNode.SelectSingleNode("//td[10]");
            var slices = teacherTdNode.InnerText.SplitAndRemoveAllSpace();
            var teacherName = string.Join(" ", slices);
            if (string.IsNullOrWhiteSpace(teacherName)) return teacherName;
            TempTeachers.Add(teacherName);
            TempTeachers = TempTeachers.Distinct().ToList();
            return teacherName;
        }

        private IEnumerable<HtmlNode> GetTrTagsWithClassLop()
        {
            try
            {
                var trTags = GetListTrTagInCalendar();
                return trTags.Where(node => node.SelectSingleNode("td").Attributes["class"].Value == "hit");
            }
            catch (IndexOutOfRangeException)
            {
                return Enumerable.Empty<HtmlNode>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException">Xảy ra khi cào thông tin môn học mà môn học đó chỉ có thông tin
        /// mô tả chứ không chứa thông tin lịch học.</exception>
        private IEnumerable<HtmlNode> GetListTrTagInCalendar()
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(_rawSoup);

            if (htmlDocument.DocumentNode.Descendants("table").Count() < 4)
            {
                throw new IndexOutOfRangeException("Không tồn tại bảng lịch");
            }
            
            var tableTbCalendar = htmlDocument.DocumentNode.Descendants("table").ToArray()[3];
            var bodyCalendar = tableTbCalendar.Descendants("tbody").ToArray()[0];
            var trTags = bodyCalendar.Descendants("tr");
            return trTags;
        }

        private static string GetSubjectDetailPageUrl(HtmlNode aTag)
        {
            return "http://courses.duytan.edu.vn/Sites/" + aTag.Attributes["href"].Value;
        }

        /// <summary>
        /// Tách mã môn từ một chuỗi.
        /// </summary>
        /// <returns>Mã môn (ví dụ CS 414)</returns>
        private static IEnumerable<string> SubjectSplit(string text)
        {
            if (text.Equals("(Không có Môn học Tiên quyết)") ||
                text.Equals("(Không có Môn học Song hành)", StringComparison.Ordinal))
            {
                yield break;
            }

            var regex = new Regex(@"(?<=\()(.*?)(?=\))");
            var matchSubject = regex.Matches(text);
            for (var i = 0; i < matchSubject.Count; ++i)
            {
                yield return matchSubject[i].Value;
            }
        }
    }
}
