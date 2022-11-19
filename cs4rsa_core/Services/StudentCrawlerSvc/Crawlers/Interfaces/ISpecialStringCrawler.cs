using System.Threading.Tasks;

namespace Cs4rsa.Services.StudentCrawlerSvc.Crawlers.Interfaces
{
    public interface ISpecialStringCrawler
    {
        public Task<string> GetSpecialString(string sessionId);
    }
}
