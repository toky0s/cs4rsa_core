using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Cs4rsa.Service.SubjectCrawler.Crawlers.Interfaces
{
    public interface ICourseHtmlGetter
    {
        Task<HtmlDocument> GetHtmlDocument(string courseId, string semesterId);
    }
}