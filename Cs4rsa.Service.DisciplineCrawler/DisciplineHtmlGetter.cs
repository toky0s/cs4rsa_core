using HtmlAgilityPack;

namespace Cs4rsa.Service.DisciplineCrawler
{
    public class DisciplineHtmlGetter : IDisciplineHtmlGetter
    {
        public HtmlDocument GetHtmlDocument(string url)
        {
            var htmlWeb = new HtmlWeb();
            return htmlWeb.Load(url);
        }
    }
}
