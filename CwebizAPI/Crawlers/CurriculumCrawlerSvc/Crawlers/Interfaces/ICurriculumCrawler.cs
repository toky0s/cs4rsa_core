/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using CwebizAPI.Crawlers.StudentCrawlerSvc.Models;

namespace CwebizAPI.Crawlers.CurriculumCrawlerSvc.Crawlers.Interfaces
{
    /// <summary>
    /// Interface CurriculumCrawler
    /// </summary>
    /// <remarks>
    /// Created Date: 21/06/2023
    /// Modified Date: 21/06/2023
    /// Author: Truong A Xin
    /// </remarks>
    public interface ICurriculumCrawler
    {
        /// <summary>
        /// Lấy thông tin mã ngành bằng Special String.
        /// </summary>
        /// <param name="specialString">Mã hash của một sinh viên</param>
        /// <returns>Thông tin mã ngành</returns>
        /// <remarks>
        /// Created Date: 21/06/2023
        /// Modified Date: 21/06/2023
        /// Author: Truong A Xin
        /// </remarks>
        Task<CrawledCurriculum> GetCurriculum(string? specialString);
    }
}
