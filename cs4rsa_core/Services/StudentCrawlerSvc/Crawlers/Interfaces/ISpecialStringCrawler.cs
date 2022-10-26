using System.Threading.Tasks;

namespace cs4rsa_core.Services.StudentCrawlerSvc.Crawlers.Interfaces
{
    public interface ISpecialStringCrawler
    {
        public Task<string> GetSpecialString(string sessionId);
    }
}
