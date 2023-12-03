using HtmlAgilityPack;

namespace Cs4rsa.Service.DisciplineCrawler
{
    public interface IDisciplineHtmlGetter
    {
        HtmlDocument GetHtmlDocument(string url);
    }
}