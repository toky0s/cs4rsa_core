using CommunityToolkit.Mvvm.ComponentModel;

using Cs4rsa.BaseClasses;
using Cs4rsa.Constants;
using Cs4rsa.Cs4rsaDatabase.Interfaces;

using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace Cs4rsa.Services.CourseSearchSvc.Crawlers
{
    /// <summary>
    /// Đảm nhiệm việc cào thông tin học kỳ và năm học.
    /// </summary>
    public partial class CourseCrawler : BaseCrawler
    {
        private readonly HtmlWeb _htmlWeb;
        private readonly IUnitOfWork _unitOfWork;
        private static bool _getYearAtFirst;

        [ObservableProperty] private string _currentYearValue;

        [ObservableProperty] private string _currentYearInfo;

        [ObservableProperty] private string _currentSemesterValue;

        [ObservableProperty] private string _currentSemesterInfo;

        public CourseCrawler(
            HtmlWeb htmlWeb,
            IUnitOfWork unitOfWork
        )
        {
            _htmlWeb = htmlWeb;
            _unitOfWork = unitOfWork;
            _getYearAtFirst = false;
        }

        /// <summary>
        /// Init Infor
        /// </summary>
        /// <exception cref="System.Net.WebException">
        /// Trong trường hợp bạn DOS server Duy Tân bằng việc cập nhật cache.
        /// </exception>
        public void InitInformation()
        {
            const string urlYearCombobox = "http://courses.duytan.edu.vn/Modules/academicprogram/ajax/LoadNamHoc.aspx?namhocname=cboNamHoc2&id=2";
            try
            {
                HtmlDocument document = _htmlWeb.Load(urlYearCombobox);

                CurrentYearInfo = GetCurrentInfo(document, true);
                CurrentYearValue = GetCurrentValue(document, true);

                string urlSemesterCombobox = $"http://courses.duytan.edu.vn/Modules/academicprogram/ajax/LoadHocKy.aspx?hockyname=cboHocKy1&namhoc={CurrentYearValue}";
                document = _htmlWeb.Load(urlSemesterCombobox);
                CurrentSemesterValue = GetCurrentValue(document, false);
                CurrentSemesterInfo = GetCurrentInfo(document, false);
                Debug.WriteLine($"Init mà không gặp lỗi", typeof(CourseCrawler).Namespace);
            }
            catch (WebException ex)
            {
                Debug.WriteLine($"Offline Mode sẽ được bật: {ex.Message}", typeof(CourseCrawler).Namespace);
                CurrentYearValue = _unitOfWork.Settings.GetBykey(VmConstants.StCurrentYearValue);
                CurrentYearInfo = _unitOfWork.Settings.GetBykey(VmConstants.StCurrentYearInfo);
                CurrentSemesterValue = _unitOfWork.Settings.GetBykey(VmConstants.StCurrentSemesterValue);
                CurrentSemesterInfo = _unitOfWork.Settings.GetBykey(VmConstants.StCurrentSemesterInfo);
            }
            finally
            {
                Debug.WriteLine($"Init hoàn tất", typeof(CourseCrawler).Namespace);
            }
        }

        private static string GetCurrentValue(
            HtmlDocument document
            , bool getYear)
        {
            IEnumerable<HtmlNode> optionElements = document
                .DocumentNode
                .Descendants()
                .Where(node => node.Name.Equals("option"));
            if (getYear && _getYearAtFirst)
            {
                return optionElements.ElementAt(1).Attributes["value"].Value;
            }
            else
            {
                return optionElements.Last().Attributes["value"].Value;
            }
        }

        private static string GetCurrentInfo(
            HtmlDocument document
            , bool getYear)
        {
            IEnumerable<HtmlNode> optionElements = document
                .DocumentNode
                .Descendants()
                .Where(node => node.Name.Equals("option"));
            if (getYear)
            {
                string years = optionElements.Last().InnerText.Trim();
                _getYearAtFirst = false;
                if (years.Contains(DateTime.Now.Year.ToString())) return years;
                years = optionElements.ElementAt(1).InnerText.Trim();
                _getYearAtFirst = true;
                return years;
            }
            else
            {
                return optionElements.Last().InnerText.Trim();
            }
        }
    }
}
