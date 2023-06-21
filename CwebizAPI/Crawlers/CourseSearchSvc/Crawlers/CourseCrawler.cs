/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using HtmlAgilityPack;

using System.Net;

namespace CwebizAPI.Crawlers.CourseSearchSvc.Crawlers
{
    /// <summary>
    /// Đảm nhiệm việc cào thông tin học kỳ và năm học.
    /// </summary>
    public class CourseCrawler : BaseCrawler
    {
        private const string MessageStartCrawl = "{Time} Start get semester informations";
        private const string MessageSuccessCrawl = "{Time} Getting semester informations is successful, year infor {YearInfo}, semester infor {SemesterInfo}";
        private const string MessageNetworkIssue = "{Time} Has network problem when try to get semester information, {ErrorMessage}";
        private const string MessageCrawlCompleted = "{Time} Getting semester informations finished";

        private readonly HtmlWeb _htmlWeb;
        private readonly ILogger<CourseCrawler> _logger;
        private static bool _getYearAtFirst;

        public string? CurrentYearValue { get; private set; }
        public string? CurrentYearInfo { get; private set; }
        public string? CurrentSemesterValue { get; private set; }
        public string? CurrentSemesterInfo { get; private set; }

        /// <summary>
        /// Khởi tạo bộ cào thông tin học kỳ
        /// </summary>
        /// <remarks>
        /// Author: Truong A Xin
        /// Created Date: 06/09/2023
        /// Modified Date:
        ///     06/09/2023:
        ///         Migrate to API.
        /// </remarks>
        /// <param name="htmlWeb">HtmlWeb</param>
        /// <param name="logger"></param>
        public CourseCrawler(
            HtmlWeb htmlWeb,
            ILogger<CourseCrawler> logger
        )
        {
            _htmlWeb = htmlWeb;
            _logger = logger;
            _getYearAtFirst = false;
        }

        /// <summary>
        /// Init Information
        /// </summary>
        /// <exception cref="WebException">
        /// Trong trường hợp bạn DOS server Duy Tân bằng việc cập nhật cache.
        /// </exception>
        public async Task InitInformation()
        {
            try
            {
                _logger.LogInformation(message: MessageStartCrawl, DateTime.UtcNow.ToLongDateString());
                const string urlYearCombobox = "http://courses.duytan.edu.vn/Modules/academicprogram/ajax/LoadNamHoc.aspx?namhocname=cboNamHoc2&id=2";
                HtmlDocument document = await _htmlWeb.LoadFromWebAsync(urlYearCombobox);

                CurrentYearInfo = GetCurrentInfo(document, true);
                CurrentYearValue = GetCurrentValue(document, true);

                string urlSemesterCombobox = $"http://courses.duytan.edu.vn/Modules/academicprogram/ajax/LoadHocKy.aspx?hockyname=cboHocKy1&namhoc={CurrentYearValue}";
                document = await _htmlWeb.LoadFromWebAsync(urlSemesterCombobox);
                CurrentSemesterValue = GetCurrentValue(document, false);
                CurrentSemesterInfo = GetCurrentInfo(document, false);
                _logger.LogInformation(message: MessageSuccessCrawl,
                                       DateTime.UtcNow.ToLongDateString(),
                                       CurrentYearInfo,
                                       CurrentSemesterInfo);
            }
            catch (WebException ex)
            {
                _logger.LogCritical(message: MessageNetworkIssue,
                                    DateTime.UtcNow.ToLongTimeString(),
                                    ex.Message);
            }
            finally
            {
                _logger.LogInformation(message: MessageCrawlCompleted,
                                       DateTime.UtcNow.ToShortTimeString());
            }
        }

        /// <summary>
        /// Lấy ra value mới nhất của Year hoặc Semester.
        /// </summary>
        /// <param name="document">HtmlDocument</param>
        /// <param name="getYear">Nếu True sẽ lấy Year, ngược lại lấy ra Semester.</param>
        /// <returns>Giá trị value mới nhất của Combobox.</returns>
        private static string GetCurrentValue(HtmlDocument document, bool getYear)
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

        /// <summary>
        /// Lấy ra thông tin chi tiết.
        /// </summary>
        /// <param name="document">HtmlDocument</param>
        /// <param name="getYear">Nếu True sẽ lấy Year, ngược lại lấy ra Semester.</param>
        /// <returns>Thông tin chi tiết mới nhất của Combobox.</returns>
        private static string GetCurrentInfo(HtmlDocument document, bool getYear)
        {
            IEnumerable<HtmlNode> optionElements = document
                .DocumentNode
                .Descendants()
                .Where(node => node.Name.Equals("option"));
            if (getYear)
            {
                IEnumerable<HtmlNode> htmlNodes = optionElements as HtmlNode[] ?? optionElements.ToArray();
                string years = htmlNodes.Last().InnerText.Trim();
                _getYearAtFirst = false;
                if (years.Contains(DateTime.Now.Year.ToString())) return years;
                years = htmlNodes.ElementAt(1).InnerText.Trim();
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
