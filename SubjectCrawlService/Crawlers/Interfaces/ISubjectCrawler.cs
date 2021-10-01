using SubjectCrawlService.BasicDatas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubjectCrawlService.Crawlers.Interfaces
{
    public interface ISubjectCrawler
    {
        Subject Crawl();
    }
}
