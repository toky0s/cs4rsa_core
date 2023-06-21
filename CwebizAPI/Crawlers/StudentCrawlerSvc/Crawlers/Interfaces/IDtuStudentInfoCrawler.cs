/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using CwebizAPI.Crawlers.StudentCrawlerSvc.Models;

namespace CwebizAPI.Crawlers.StudentCrawlerSvc.Crawlers.Interfaces
{
    /// <summary>
    /// Interface Duy Tan University Student Information Crawler
    /// </summary>
    /// <remarks>
    /// Created Date: 21/06/2023
    /// Modified Date: 21/06/2023
    /// Author: Truong A Xin
    /// </remarks>
    public interface IDtuStudentInfoCrawler
    {
        /// <summary>
        /// Cào thông tin mã ngành và thông tin sinh viên.
        /// </summary>
        /// <param name="specialString">Mã hash của sinh viên.</param>
        /// <returns>CrawledCurriculum và DtuStudent.</returns>
        Task<Tuple<CrawledCurriculum, DtuStudent>> Crawl(string? specialString);

        /// <summary>
        /// Tải ảnh hồ sơ của một sinh viên.
        /// </summary>
        /// <param name="studentCode">Mã sinh viên</param>
        /// <returns>Trả về đường dẫn lưu ảnh nếu tải thành công. Ngược lại trả về chuỗi rỗng</returns>
        Task<string> DownloadProfileImg(string studentCode);
    }
}
