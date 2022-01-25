using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentCrawlerService.Crawlers.Interfaces
{
    public interface ISpecialStringCrawler
    {
        public Task<string> GetSpecialString(string sessionId);
    }
}
