/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using CwebizAPI.Crawlers.CurriculumCrawlerSvc.Crawlers.Interfaces;
using CwebizAPI.Crawlers.StudentCrawlerSvc.Crawlers.Interfaces;
using CwebizAPI.Utils;

using HtmlAgilityPack;

using System.Diagnostics;
using System.Globalization;
using CwebizAPI.Crawlers.StudentCrawlerSvc.Models;
using CwebizAPI.Services.Interfaces;

namespace CwebizAPI.Crawlers.StudentCrawlerSvc.Crawlers
{
    /// <summary>
    /// Bộ cào thông tin sinh viên.
    /// </summary>
    /// <remarks>
    /// 23/1/2022 Version 2 DTU Student Info Crawler
    /// - Fix XPath.
    /// - Fix GetByPaging Image.
    ///
    /// 16/06/2023 Migrate to Cwebiz
    /// - Change businesses.
    /// - Separate logic.
    /// - Chỉnh sửa tài liệu.
    /// </remarks>
    public class DtuStudentInfoCrawlerV2 : BaseCrawler, IDtuStudentInfoCrawler
    {
        private readonly ICurriculumCrawler _curriculumCrawler;
        private readonly HtmlWeb _htmlWeb;
        private readonly ImageDownloader _imageDownloader;
        private readonly IImageStorageSvc _imageStorageSvc;

        public DtuStudentInfoCrawlerV2(
            ICurriculumCrawler curriculumCrawler,
            ImageDownloader imageDownloader,
            HtmlWeb htmlWeb, 
            IImageStorageSvc imageStorageSvc
        )
        {
            _htmlWeb = htmlWeb;
            _imageStorageSvc = imageStorageSvc;
            _curriculumCrawler = curriculumCrawler;
            _imageDownloader = imageDownloader;
        }
        public async Task<Tuple<CrawledCurriculum, DtuStudent>> Crawl(string? specialString)
        {
            string url = $"https://mydtu.duytan.edu.vn/Modules/mentor/WarningDetail.aspx?t={GetTimeFromEpoch()}&stid={specialString}";
            HtmlDocument doc = await _htmlWeb.LoadFromWebAsync(url);
            HtmlNode docNode = doc.DocumentNode;

            HtmlNode nameNode = docNode.SelectSingleNode("//tr/td[2]/table/tr[1]/td[2]/strong");
            HtmlNode studentIdNode = docNode.SelectSingleNode("//tr/td[2]/table/tr[2]/td[2]");
            HtmlNode birthdayNode = docNode.SelectSingleNode("//tr[3]/td[2]");
            HtmlNode cmndNode = docNode.SelectSingleNode("//tr[4]/td[2]");
            HtmlNode emailNode = docNode.SelectSingleNode("//tr[5]/td[2]");
            HtmlNode phoneNumberNode = docNode.SelectSingleNode("//tr[6]/td[2]");
            HtmlNode addressNode = docNode.SelectSingleNode("//tr[7]/td[2]");
            string imageId = $"imgDetail_{studentIdNode.InnerText}";
            HtmlNode imageNode = docNode.SelectSingleNode($"//*[@id=\"{imageId}\"]");

            string name = nameNode.InnerText;
            string studentId = studentIdNode.InnerText;

            string sBirthday = StringHelper.ParseDateTime(birthdayNode.InnerText);
            if (!DateTime.TryParseExact(sBirthday, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime birthday))
            {
                if (!DateTime.TryParseExact(sBirthday, "dd/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out birthday))
                {
                    if (!DateTime.TryParseExact(sBirthday, "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out birthday))
                    {
                        DateTime.TryParseExact(sBirthday, "d/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out birthday);
                    }
                }
            }

            string cmnd = cmndNode.InnerText;
            string emails = emailNode.InnerText;
            string phoneNumber = phoneNumberNode.InnerText;
            string address = addressNode.InnerText.SuperCleanString();

            string imageSrcData = imageNode.Attributes["src"].Value;
            Uri imageSrc = new(imageSrcData);
            string? imgPath = await LoadImage(imageSrc, studentId);

            CrawledCurriculum crawledCurriculum = await _curriculumCrawler.GetCurriculum(specialString);

            DtuStudent dtuStudent = new()
            {
                Name = name,
                StudentId = studentId,
                SpecialString = specialString,
                BirthDay = birthday,
                Cmnd = cmnd,
                Emails = emails.GetEmails(),
                PhoneNumber = phoneNumber,
                Address = address,
                AvatarImgPath = imgPath,
            };

            return new Tuple<CrawledCurriculum, DtuStudent>(crawledCurriculum, dtuStudent);
        }

        /// <summary>
        /// Tải hình ảnh hồ sơ sinh viên.
        /// </summary>
        /// <remarks>
        /// Nếu hình ảnh tôn tại, trả về đường dẫn tới file.
        /// Ngược lại tải ảnh về trả về đường dẫn tới file.
        /// </remarks>
        /// <param name="uri">Đường dẫn tới hình ảnh.</param>
        /// <param name="studentId">Mã sinh viên.</param>
        /// <returns>Đường dẫn tới nơi lưu trữ ảnh sinh viên trên hệ thống Cwebiz</returns>
        private async Task<string?> LoadImage(Uri uri, string studentId)
        {
            try
            {
                using HttpClient client = new();
                using HttpResponseMessage response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                Stream stream = await response.Content.ReadAsStreamAsync();
                return _imageStorageSvc.SaveStudentImageByStream(stream, studentId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to load the image: {0}", ex.Message);
            }

            return null;
        }

        public async Task<string> DownloadProfileImg(string studentCode)
        {
            string url = $"http://hfs1.duytan.edu.vn/Upload/dichvu/sv_{studentCode}_01.jpg";
            return await _imageDownloader.DownloadImage(url, studentCode);
        }
    }
}
