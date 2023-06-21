/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

namespace CwebizAPI.Crawlers.StudentCrawlerSvc.Crawlers.Interfaces
{
    /// <summary>
    /// Interface Special String Crawler
    /// </summary>
    /// <remarks>
    /// Created Date: 21/06/2023
    /// Modified Date: 21/06/2023
    /// Author: Truong A Xin
    /// </remarks>
    public interface ISpecialStringCrawler
    {
        /// <summary>
        /// Lấy ra Special String của một sinh viên dựa vào Session ID.
        /// </summary>
        /// <param name="sessionId">ASP.NET_SessionID.</param>
        /// <returns>Mã hash.</returns>
        public Task<string?> GetSpecialString(string sessionId);
    }
}
