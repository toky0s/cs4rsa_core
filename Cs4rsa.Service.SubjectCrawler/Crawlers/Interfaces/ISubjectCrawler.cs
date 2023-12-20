using System.Threading.Tasks;
using Cs4rsa.Service.SubjectCrawler.DataTypes;

namespace Cs4rsa.Service.SubjectCrawler.Crawlers.Interfaces
{
    public interface ISubjectCrawler
    {
        Task<Subject> Crawl(string courseId, string semesterId);
        Task<Subject> Crawl(string cache);
    }
}
