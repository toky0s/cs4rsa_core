using Cs4rsa.Service.CourseCrawler.Interfaces;
using HtmlAgilityPack;

namespace Cs4rsa.Service.CourseCrawler.Crawlers
{
    public class SemesterHtmlGetter : ISemesterHtmlGetter
    {
        public HtmlDocument GetHtmlDocument(string url)
        {
            var htmlWeb = new HtmlWeb();
            return htmlWeb.Load(url);
        }
    }
}