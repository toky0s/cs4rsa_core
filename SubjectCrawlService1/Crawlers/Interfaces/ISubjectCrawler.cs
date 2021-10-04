using SubjectCrawlService1.BasicDatas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubjectCrawlService1.Crawlers.Interfaces
{
    public interface ISubjectCrawler
    {
        Subject Crawl(string discipline, string keyword1);
    }
}
