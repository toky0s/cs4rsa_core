using CommunityToolkit.Mvvm.ComponentModel;

using Cs4rsa.BaseClasses;
using Cs4rsa.Services.CourseSearchSvc.Crawlers.Interfaces;
using Cs4rsa.Services.CourseSearchSvc.DataTypes;
using Cs4rsa.Settings.Interfaces;

using HtmlAgilityPack;

using System.Collections.Generic;
using System.Linq;

namespace Cs4rsa.Services.CourseSearchSvc.Crawlers
{
    /// <summary>
    /// Đảm nhiệm việc cào thông tin học kỳ và năm học.
    /// </summary>
    public partial class CourseCrawler : BaseCrawler, ICourseCrawler
    {
        private readonly HtmlWeb _htmlWeb;
        private readonly ISetting _setting;

        [ObservableProperty]
        public string _currentYearValue;

        [ObservableProperty]
        public string _currentYearInfo;

        [ObservableProperty]
        public string _currentSemesterValue;

        [ObservableProperty]
        public string _currentSemesterInfo;

        public CourseCrawler(
            HtmlWeb htmlWeb,
            ISetting setting
        )
        {
            _htmlWeb = htmlWeb;
            _setting = setting;
        }

        public void InitInfor()
        {
            try
            {
                string URL_YEAR_COMBOBOX = "http://courses.duytan.edu.vn/Modules/academicprogram/ajax/LoadNamHoc.aspx?namhocname=cboNamHoc2&id=2";
                HtmlDocument document = _htmlWeb.Load(URL_YEAR_COMBOBOX);

                CurrentYearValue = GetCurrentValue(document);
                CurrentYearInfo = GetCurrentInfo(document);

                string URL_SEMESTER_COMBOBOX = $"http://courses.duytan.edu.vn/Modules/academicprogram/ajax/LoadHocKy.aspx?hockyname=cboHocKy1&namhoc={CurrentYearValue}";
                document = _htmlWeb.Load(URL_SEMESTER_COMBOBOX);
                CurrentSemesterValue = GetCurrentValue(document);
                CurrentSemesterInfo = GetCurrentInfo(document);

                _setting.CurrentSetting.CurrentYearValue = CurrentYearValue;
                _setting.CurrentSetting.CurrentSemesterValue = CurrentSemesterValue;
                _setting.CurrentSetting.CurrentYear = CurrentYearInfo;
                _setting.CurrentSetting.CurrentSemester = CurrentSemesterInfo;
                _setting.Save();
            }
            catch
            {
                CurrentYearInfo = _setting.CurrentSetting.CurrentYear;
                CurrentSemesterInfo = _setting.CurrentSetting.CurrentSemester;
                CurrentYearValue = _setting.CurrentSetting.CurrentYearValue;
                CurrentSemesterValue = _setting.CurrentSetting.CurrentSemesterValue;
            }
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

        private IEnumerable<CourseYear> GetCourseYears(HtmlDocument document)
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

        private IEnumerable<CourseSemester> GetCourseSemesters(string yearValue)
        {
            string urlTemplate = "http://courses.duytan.edu.vn/Modules/academicprogram/ajax/LoadHocKy.aspx?hockyname=cboHocKy1&namhoc={0}";
            string url = string.Format(urlTemplate, yearValue);
            HtmlDocument document = _htmlWeb.Load(url);

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
