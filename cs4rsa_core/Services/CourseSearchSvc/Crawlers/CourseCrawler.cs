using CommunityToolkit.Mvvm.ComponentModel;

using Cs4rsa.BaseClasses;
using Cs4rsa.Constants;
using Cs4rsa.Cs4rsaDatabase.Interfaces;

using HtmlAgilityPack;

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
            IUnitOfWork unitOfWork
        )
        {
            _htmlWeb = htmlWeb;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Init Infor
        /// </summary>
        /// <exception cref="System.Net.WebException">
        /// Trong trường hợp bạn DOS server Duy Tân bằng việc cập nhật cache.
        /// </exception>
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
    }
}
