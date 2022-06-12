using HelperService;

using HtmlAgilityPack;

using StudentCrawlerService.Crawlers.Interfaces;

using System;
using System.Threading.Tasks;

namespace StudentCrawlerService.Crawlers
{
    public class SpecialStringCrawlerV2 : ISpecialStringCrawler
    {
        public async Task<string> GetSpecialString(string sessionId)
        {
            string time = Helpers.GetTimeFromEpoch();
            string url = $"https://mydtu.duytan.edu.vn/Modules/mentor/ver2/ajax/StudentStudyingWarning_List.aspx?t={time}&loaiCanhBao=0&canhBao=null&mucDo=0&masv=&tensv=&academicid=0&instructorid=&cp=1";
            string html = await DtuPageCrawler.GetHtml(sessionId, url);
            HtmlDocument htmlDocument = new();
            htmlDocument.LoadHtml(html);
            HtmlNode spanTag = htmlDocument.DocumentNode.SelectSingleNode("//*/span");
            if (spanTag != null)
            {
                string onClickValue = spanTag.Attributes["onclick"].Value;
                string[] split = onClickValue.Split(new string[] { "'" }, StringSplitOptions.RemoveEmptyEntries);
                string specialString = split[1];
                return specialString;
            }
            return null;
        }
    }
}
