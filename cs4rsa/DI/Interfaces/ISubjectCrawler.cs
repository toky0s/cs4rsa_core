using cs4rsa.BasicData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.DI.Interfaces
{
    public interface ISubjectCrawler
    {
        Subject Crawl(string discipline, string keyword1);
        Subject Crawl(string courseId);
    }
}
