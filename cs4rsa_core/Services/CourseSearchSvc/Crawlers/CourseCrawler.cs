using cs4rsa_core.Services.CourseSearchSvc.Crawlers.Interfaces;
using cs4rsa_core.Services.CourseSearchSvc.DataTypes;

using HtmlAgilityPack;

using System.Collections.Generic;
using System.Linq;

namespace cs4rsa_core.Services.CourseSearchSvc.Crawlers
{
    /// <summary>
    /// Đảm nhiệm việc cào thông tin học kỳ và năm học.
    /// </summary>
    public class CourseCrawler : ICourseCrawler
    {
        public string CurrentYearValue { get; }
        public string CurrentYearInfo { get; }
        public string CurrentSemesterValue { get; }
        public string CurrentSemesterInfo { get; }
        public IEnumerable<CourseYear> CourseYears { get; }

        public CourseCrawler()
        {
            HtmlWeb htmlWeb = new();
            string URL_YEAR_COMBOBOX = "http://courses.duytan.edu.vn/Modules/academicprogram/ajax/LoadNamHoc.aspx?namhocname=cboNamHoc2&id=2";
            HtmlDocument document = htmlWeb.Load(URL_YEAR_COMBOBOX);
            CourseYears = GetCourseYears(document);

            CurrentYearValue = GetCurrentValue(document);
            CurrentYearInfo = GetCurrentInfo(document);

            string URL_SEMESTER_COMBOBOX = $"http://courses.duytan.edu.vn/Modules/academicprogram/ajax/LoadHocKy.aspx?hockyname=cboHocKy1&namhoc={CurrentYearValue}";
            document = htmlWeb.Load(URL_SEMESTER_COMBOBOX);
            CurrentSemesterValue = GetCurrentValue(document);
            CurrentSemesterInfo = GetCurrentInfo(document);
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

        private static IEnumerable<CourseYear> GetCourseYears(HtmlDocument document)
        {
            IEnumerable<HtmlNode> optionElements = document.DocumentNode
                .Descendants()
                .Where(node => node.Name == "option" && node.Attributes["value"].Value != "0");
            foreach (HtmlNode node in optionElements)
            {
                string name = node.InnerText.Trim();
                string value = node.Attributes["value"].Value;
                IEnumerable<CourseSemester> courseSemesters = GetCourseSemesters(value);
                CourseYear courseYear = new() { Name = name, Value = value, CourseSemesters = courseSemesters };
                yield return courseYear;
            }
        }

        private static IEnumerable<CourseSemester> GetCourseSemesters(string yearValue)
        {
            HtmlWeb htmlWeb = new();
            string urlTemplate = "http://courses.duytan.edu.vn/Modules/academicprogram/ajax/LoadHocKy.aspx?hockyname=cboHocKy1&namhoc={0}";
            string url = string.Format(urlTemplate, yearValue);
            HtmlDocument document = htmlWeb.Load(url);

            IEnumerable<HtmlNode> optionElements = document.DocumentNode.Descendants()
                .Where(node => node.Name == "option" && node.Attributes["value"].Value != "0");
            foreach (HtmlNode node in optionElements)
            {
                string name = node.InnerText.Trim();
                string value = node.Attributes["value"].Value;
                CourseSemester courseSemester = new() { Name = name, Value = value };
                yield return courseSemester;
            }
        }

        public string GetCurrentSemesterValue()
        {
            return CurrentSemesterValue;
        }

        public string GetCurrentSemesterInfo()
        {
            return CurrentSemesterInfo;
        }

        public string GetCurrentYearValue()
        {
            return CurrentYearValue;
        }

        public string GetCurrentYearInfo()
        {
            return CurrentYearInfo;
        }
    }

}
