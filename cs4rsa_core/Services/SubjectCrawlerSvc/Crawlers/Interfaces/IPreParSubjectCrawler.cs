using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cs4rsa.Services.SubjectCrawlerSvc.Crawlers.Interfaces
{
    public interface IPreParSubjectCrawler
    {
        Task<Tuple<IEnumerable<string>, IEnumerable<string>>> Run(string courseId, bool isUseCache);
    }
}
