using SubjectCrawlService1.DataTypes;

using System.Threading.Tasks;

namespace SubjectCrawlService1.Crawlers.Interfaces
{
    public interface ISubjectCrawler
    {
        Task<Subject> Crawl(string discipline, string keyword1);
        Task<Subject> Crawl(ushort courseId);
    }
}
