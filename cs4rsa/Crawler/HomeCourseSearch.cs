using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using cs4rsa.Properties;
using cs4rsa.Helpers;
using cs4rsa.BasicData;

namespace cs4rsa.Crawler
{
    /// <summary>
    /// <para>Class này bao gồm các method liên quan đến tìm kiếm thông tin Học Kỳ, 
    /// Năm học và Thông tin môn học được Crawl từ web.</para>
    /// </summary>
    public class HomeCourseSearch
    {
        private static readonly HomeCourseSearch instance = new HomeCourseSearch();

        private string _currentYearValue;
        private string _currentYearInfo;
        private string _currentSemesterValue;
        private string _currentSemesterInfo;

        public string CurrentYearValue => _currentYearValue;
        public string CurrentYearInfo => _currentYearInfo;
        public string CurrentSemesterValue => _currentSemesterValue;
        public string CurrentSemesterInfo => _currentSemesterInfo;

        private List<CourseYear> _courseYears;
        public List<CourseYear> CourseYears => _courseYears;

        private HomeCourseSearch()
        {
            HtmlWeb htmlWeb = new HtmlWeb();
            string URL_YEAR_COMBOBOX = "http://courses.duytan.edu.vn/Modules/academicprogram/ajax/LoadNamHoc.aspx?namhocname=cboNamHoc2&id=2";
            HtmlDocument document = htmlWeb.Load(URL_YEAR_COMBOBOX);
            _courseYears = GetCourseYears(document);

            _currentYearValue = GetCurrentValue(document);
            _currentYearInfo = GetCurrentInfo(document);

            string URL_SEMESTER_COMBOBOX = string.Format("http://courses.duytan.edu.vn/Modules/academicprogram/ajax/LoadHocKy.aspx?hockyname=cboHocKy1&namhoc={0}", _currentYearValue);
            document = htmlWeb.Load(URL_SEMESTER_COMBOBOX);
            _currentSemesterValue = GetCurrentValue(document);
            _currentSemesterInfo = GetCurrentInfo(document);
        }

        public static HomeCourseSearch GetInstance()
        {
            return instance;
        }

        private string GetCurrentValue(HtmlDocument document)
        {
            IEnumerable<HtmlNode> optionElements = document.DocumentNode.Descendants().Where(node => node.Name == "option");
            return optionElements.Last().Attributes["value"].Value;
        }

        private string GetCurrentInfo(HtmlDocument document)
        {
            IEnumerable<HtmlNode> optionElements = document.DocumentNode.Descendants().Where(node => node.Name == "option");
            return optionElements.Last().InnerText.Trim();
        }

        private List<CourseYear> GetCourseYears(HtmlDocument document)
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
                CourseYear courseYear = new CourseYear(name, value, courseSemesters);
                courseYears.Add(courseYear);
            }
            return courseYears;
        }

        private List<CourseSemester> GetCourseSemesters(string yearValue)
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
                CourseSemester courseSemester = new CourseSemester(name, value);
                courseSemesters.Add(courseSemester);
            }
            return courseSemesters;
        }
    }
}

