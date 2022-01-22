using CourseSearchDLL.Crawlers.Interfaces;
using CourseSearchDLL.DataTypes;
using HelperService;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseSearchDLL.Crawlers
{
    /// <summary>
    /// Đảm nhiệm việc cào thông tin học kỳ và năm học.
    /// </summary>
    public class CourseCrawler : ICourseCrawler
    {
        private string _currentYearValue;
        private string _currentYearInfo;
        private string _currentSemesterValue;
        private string _currentSemesterInfo;

        private List<CourseYear> _courseYears;

        private CourseCrawler(List<CourseYear> courseYears, string currentYearValue,
            string currentYearInfo, string currentSemesterValue, string currentSemesterInfo)
        {
            _courseYears = courseYears;
            _currentSemesterInfo = currentSemesterInfo;
            _currentSemesterValue = currentSemesterValue;
            _currentYearValue = currentYearValue;
            _currentYearInfo = currentYearInfo;
        }

        public static async Task<CourseCrawler> GetCourseCrawler()
        {
            string URL_YEAR_COMBOBOX = $"http://courses.duytan.edu.vn/Modules/academicprogram/ajax/LoadNamHoc.aspx?namhocname=cboNamHoc2&id=2&t={Helpers.GetTimeFromEpoch()}";
            HtmlWeb htmlWeb = new();
            HtmlDocument document = await htmlWeb.LoadFromWebAsync(URL_YEAR_COMBOBOX);
            List<CourseYear> courseYears = GetCourseYears(document);

            string currentYearValue = GetCurrentValue(document);
            string currentYearInfo = GetCurrentInfo(document);

            string URL_SEMESTER_COMBOBOX = $"http://courses.duytan.edu.vn/Modules/academicprogram/ajax/LoadHocKy.aspx?hockyname=cboHocKy1&namhoc={currentYearValue}";
            document = await htmlWeb.LoadFromWebAsync(URL_SEMESTER_COMBOBOX);
            string currentSemesterValue = GetCurrentValue(document);
            string currentSemesterInfo = GetCurrentInfo(document);
            return new CourseCrawler(courseYears, currentYearValue, currentYearInfo,
                currentSemesterValue, currentSemesterInfo);
        }

        private static string GetCurrentValue(HtmlDocument document)
        {
            IEnumerable<HtmlNode> optionElements = document.DocumentNode.Descendants().Where(node => node.Name == "option");
            return optionElements.Last().Attributes["value"].Value;
        }

        private static string GetCurrentInfo(HtmlDocument document)
        {
            IEnumerable<HtmlNode> optionElements = document.DocumentNode.Descendants().Where(node => node.Name == "option");
            return optionElements.Last().InnerText.Trim();
        }

        private static List<CourseYear> GetCourseYears(HtmlDocument document)
        {
            List<CourseYear> courseYears = new List<CourseYear>();
            List<HtmlNode> optionElements = document.DocumentNode.Descendants()
                .Where(node => node.Name == "option")
                .ToList();
            optionElements.RemoveAt(0);
            foreach (HtmlNode node in optionElements)
            {
                string name = node.InnerText.Trim();
                string value = node.Attributes["value"].Value;
                List<CourseSemester> courseSemesters = GetCourseSemesters(value);
                CourseYear courseYear = new() { Name = name, Value = value, CourseSemesters = courseSemesters };
                courseYears.Add(courseYear);
            }
            return courseYears;
        }

        private static List<CourseSemester> GetCourseSemesters(string yearValue)
        {
            HtmlWeb htmlWeb = new HtmlWeb();
            string urlTemplate = "http://courses.duytan.edu.vn/Modules/academicprogram/ajax/LoadHocKy.aspx?hockyname=cboHocKy1&namhoc={0}";
            string url = string.Format(urlTemplate, yearValue);
            HtmlDocument document = htmlWeb.Load(url);

            List<CourseSemester> courseSemesters = new List<CourseSemester>();
            List<HtmlNode> optionElements = document.DocumentNode.Descendants()
                .Where(node => node.Name == "option")
                .ToList();
            optionElements.RemoveAt(0);
            foreach (HtmlNode node in optionElements)
            {
                string name = node.InnerText.Trim();
                string value = node.Attributes["value"].Value;
                CourseSemester courseSemester = new() { Name = name, Value = value };
                courseSemesters.Add(courseSemester);
            }
            return courseSemesters;
        }

        public string GetCurrentSemesterValue()
        {
            return _currentSemesterValue;
        }

        public string GetCurrentSemesterInfo()
        {
            return _currentSemesterInfo;
        }

        public string GetCurrentYearValue()
        {
            return _currentYearValue;
        }

        public string GetCurrentYearInfo()
        {
            return _currentYearInfo;
        }

        public List<CourseYear> GetCourseYears()
        {
            return _courseYears;
        }
    }

}
