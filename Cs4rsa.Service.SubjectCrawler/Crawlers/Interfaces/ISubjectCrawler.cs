using System.Threading.Tasks;
using Cs4rsa.Service.SubjectCrawler.DataTypes;

namespace Cs4rsa.Service.SubjectCrawler.Crawlers.Interfaces
{
    public interface ISubjectCrawler
    {
        /// <summary>
        /// Lấy thông tin môn học từ https://courses.duytan.edu.vn/
        /// </summary>
        /// <param name="courseId">Course ID</param>
        /// <param name="semesterId">Semester ID</param>
        /// <returns>Trả về thông tin môn học và HTML</returns>
        Task<(Subject, string)> Crawl(string courseId, string semesterId);
        Subject CrawlFromCache(string cache, string courseId);
    }
}
