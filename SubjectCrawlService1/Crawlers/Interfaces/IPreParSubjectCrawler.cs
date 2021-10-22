using SubjectCrawlService1.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubjectCrawlService1.Crawlers.Interfaces
{
    public interface IPreParSubjectCrawler
    {
        public bool IsAvailableSubject { get; set; }
        Task<PreParContainer> Run(string courseId);
        Task<PreParContainer> Run(string courseId, string sessionId);
    }
}
