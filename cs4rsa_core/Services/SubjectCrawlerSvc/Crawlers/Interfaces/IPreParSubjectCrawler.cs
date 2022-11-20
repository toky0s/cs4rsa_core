using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes;

using System.Threading.Tasks;

namespace Cs4rsa.Services.SubjectCrawlerSvc.Crawlers.Interfaces
{
    public interface IPreParSubjectCrawler
    {
        public bool IsAvailableSubject { get; set; }
        Task<PreParContainer> Run(string courseId);
        Task<PreParContainer> Run(string courseId, string sessionId);
    }
}
