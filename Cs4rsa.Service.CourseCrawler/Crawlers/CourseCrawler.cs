using System;
using System.Linq;
using Cs4rsa.Service.CourseCrawler.Interfaces;
using HtmlAgilityPack;

namespace Cs4rsa.Service.CourseCrawler.Crawlers
{
    /// <summary>
    /// Đảm nhiệm việc cào thông tin học kỳ và năm học.
    /// </summary>
    public class CourseCrawler : ICourseCrawler
    {
        private readonly ISemesterHtmlGetter _semesterHtmlGetter;
        private static bool _getYearAtFirst;

        public CourseCrawler(ISemesterHtmlGetter semesterHtmlGetter)
        {
            _semesterHtmlGetter = semesterHtmlGetter;
            _getYearAtFirst = false;
        }

        /// <summary>
        /// Init Info
        /// </summary>
        public void GetInfo(
            out string yearInfo
            , out string yearValue
            , out string semesterInfo
            , out string semesterValue)
        {
            const string urlYearCombobox = "http://courses.duytan.edu.vn/Modules/academicprogram/ajax/LoadNamHoc.aspx?namhocname=cboNamHoc2&id=2";
            var document = _semesterHtmlGetter.GetHtmlDocument(urlYearCombobox);

            yearInfo = GetCurrentInfo(document, true);
            yearValue = GetCurrentValue(document, true);

            var urlSemesterCombobox = $"http://courses.duytan.edu.vn/Modules/academicprogram/ajax/LoadHocKy.aspx?hockyname=cboHocKy1&namhoc={yearValue}";
            document = _semesterHtmlGetter.GetHtmlDocument(urlSemesterCombobox);
            semesterValue = GetCurrentValue(document, false);
            semesterInfo = GetCurrentInfo(document, false);
        }

        private static string GetCurrentValue(
            HtmlDocument document
            , bool getYear)
        {
            var optionElements = document
                .DocumentNode
                .Descendants()
                .Where(node => node.Name.Equals("option"));
            if (getYear && _getYearAtFirst)
            {
                return optionElements.ElementAt(1).Attributes["value"].Value;
            }

            return optionElements.Last().Attributes["value"].Value;
        }

        private static string GetCurrentInfo(
            HtmlDocument document
            , bool getYear)
        {
            var optionElements = document
                .DocumentNode
                .Descendants()
                .Where(node => node.Name.Equals("option"))
                .ToArray();
            if (getYear)
            {
                var years = optionElements.Last().InnerText.Trim();
                _getYearAtFirst = false;
                if (years.Contains(DateTime.Now.Year.ToString()))
                {
                    return years;
                }
                years = optionElements.ElementAt(1).InnerText.Trim();
                _getYearAtFirst = true;
                return years;
            }

            return optionElements.Last().InnerText.Trim();
        }
    }
}
