using cs4rsa_core.Services.StudentCrawlerSvc.Crawlers.Interfaces;
using cs4rsa_core.Utils;

using HtmlAgilityPack;

using System;
using System.Threading.Tasks;

namespace cs4rsa_core.Services.StudentCrawlerSvc.Crawlers
{
    /// <summary>
    /// Trình cào này thực hiện lấy chuỗi đặc biệt từ session id được truyền vào.
    /// </summary>
    public class SpecialStringCrawler : ISpecialStringCrawler
    {
        public async Task<string> GetSpecialString(string sessionId)
        {
            string urlAddress = "https://mydtu.duytan.edu.vn/sites/index.aspx?p=home_studyingwarning&functionid=113";
            string html = await DtuPageCrawler.GetHtml(sessionId, urlAddress);
            HtmlDocument htmlDocument = new();
            htmlDocument.LoadHtml(html);

            HtmlNode spanTag = htmlDocument.DocumentNode.SelectSingleNode("//*[@id=\"canhbaohoctap\"]/div[2]/table/tbody/tr/td[3]/span");
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
