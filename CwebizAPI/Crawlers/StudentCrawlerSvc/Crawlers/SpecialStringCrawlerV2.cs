/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using CwebizAPI.Crawlers.StudentCrawlerSvc.Crawlers.Interfaces;

using HtmlAgilityPack;
namespace CwebizAPI.Crawlers.StudentCrawlerSvc.Crawlers
{
    /// <summary>
    /// Bộ cào Special String.
    /// </summary>
    /// <remarks>
    /// Trả về null thì mặc định là gặp lỗi trong quá trình parse.
    /// 
    /// Created Date: 16/06/2023
    /// Updated Date: 16/06/2023
    /// Author: truong a xin
    /// </remarks>
    public class SpecialStringCrawlerV2 : BaseCrawler, ISpecialStringCrawler
    {
        public async Task<string?> GetSpecialString(string sessionId)
        {
            string url = $"https://mydtu.duytan.edu.vn/Modules/mentor/ver2/ajax/StudentStudyingWarning_List.aspx?t={GetTimeFromEpoch()}&loaiCanhBao=0&canhBao=null&mucDo=0&masv=&tensv=&academicid=0&instructorid=&cp=1";
            string html = await DtuPageCrawler.GetHtml(sessionId, url);
            HtmlDocument htmlDocument = new();
            htmlDocument.LoadHtml(html);
            HtmlNode? spanTag = htmlDocument.DocumentNode.SelectSingleNode("//*/span");
            if (spanTag == null) return default;
            string? onClickValue = spanTag.Attributes["onclick"].Value;
            string[] split = onClickValue.Split(new string[] { "'" }, StringSplitOptions.RemoveEmptyEntries);
            string specialString = split[1];
            return specialString;
        }
    }
}
