using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using cs4rsa.Properties;
using cs4rsa.Helpers;

namespace cs4rsa.Crawler
{
    /// <summary>
    /// <para>Class này bao gồm các method liên quan đến tìm kiếm thông tin Học Kỳ, Năm học và Thông tin môn học được Crawl từ web.</para>
    /// </summary>
    public class HomeCourseSearch
    {
        private static readonly HomeCourseSearch instance = new HomeCourseSearch();

        private static string currentYearValue;
        private static string currentYearInfo;
        private static string currentSemesterValue;
        private static string currentSemesterInfo;

        public string CurrentYearValue => currentYearValue;

        public string CurrentYearInfo => currentYearInfo;

        public string CurrentSemesterValue => currentSemesterValue;

        public string CurrentSemesterInfo => currentSemesterInfo;

        private HomeCourseSearch()
        {
            HtmlWeb htmlWeb = new HtmlWeb();
            string URL_YEAR_COMBOBOX = "http://courses.duytan.edu.vn/Modules/academicprogram/ajax/LoadNamHoc.aspx?namhocname=cboNamHoc2&id=2";
            HtmlDocument document = htmlWeb.Load(URL_YEAR_COMBOBOX);
            currentYearValue = GetCurrentValue(document);
            currentYearInfo = GetCurrentInfo(document);

            string URL_SEMESTER_COMBOBOX = string.Format("http://courses.duytan.edu.vn/Modules/academicprogram/ajax/LoadHocKy.aspx?hockyname=cboHocKy1&namhoc={0}", currentYearValue);
            document = htmlWeb.Load(URL_SEMESTER_COMBOBOX);
            currentSemesterValue = GetCurrentValue(document);
            currentSemesterInfo = GetCurrentInfo(document);
        }

        public static HomeCourseSearch GetInstance()
        {
            return instance;
        }

        private static string GetCurrentValue(HtmlDocument document)
        {
            var optionElements = document.DocumentNode.Descendants().Where(n => n.Name == "option");
            return optionElements.Last().Attributes["value"].Value;
        }

        private static string GetCurrentInfo(HtmlDocument document)
        {
            var optionElements = document.DocumentNode.Descendants().Where(node => node.Name == "option");
            return optionElements.Last().InnerText.Trim();
        }
    }
}

