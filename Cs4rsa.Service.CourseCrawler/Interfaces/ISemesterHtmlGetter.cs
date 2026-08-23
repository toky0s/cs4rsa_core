using HtmlAgilityPack;

namespace Cs4rsa.Service.CourseCrawler.Interfaces
{
    public interface ISemesterHtmlGetter
    {
        HtmlDocument GetHtmlDocument(string url);
    }
}