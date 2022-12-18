using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes;

using System.Threading.Tasks;

namespace Cs4rsa.Services.SubjectCrawlerSvc.Crawlers.Interfaces
{
    public interface IPreParSubjectCrawler
    {
        Task<PreParContainer> Run(string courseId, bool isUseCache);
        Task<PreParContainer> Run(string courseId, string sessionId);
    }
}
