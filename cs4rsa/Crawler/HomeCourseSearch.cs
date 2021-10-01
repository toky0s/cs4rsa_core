using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using cs4rsa.Properties;
using cs4rsa.Helpers;
using cs4rsa.BasicData;
using cs4rsa.DI.Interfaces;

namespace cs4rsa.Crawler
{
    /// <summary>
    /// <para>Class này bao gồm các method liên quan đến tìm kiếm thông tin Học Kỳ, 
    /// Năm học và Thông tin môn học được Crawl từ web.</para>
    /// </summary>
    public class HomeCourseSearch : ICourseCrawler
    {
        public string CurrentYearValue { get; }
        public string CurrentYearInfo { get; }
        public string CurrentSemesterValue { get; }
        public string CurrentSemesterInfo { get; }
        public List<CourseYear> CourseYears { get; }

        public HomeCourseSearch()
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

