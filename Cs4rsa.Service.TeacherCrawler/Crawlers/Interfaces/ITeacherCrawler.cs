using System.Threading.Tasks;
using Cs4rsa.Service.TeacherCrawler.Models;

namespace Cs4rsa.Service.TeacherCrawler.Crawlers.Interfaces
{
    public interface ITeacherCrawler
    {
        /// <summary>
        /// Cào thông tin của một giảng viên
        /// </summary>
        /// <param name="url">Url tới trang chi tiết giảng viên</param>
        /// <param name="courseId">
        /// Course ID của môn học, cùng với Teacher ID sẽ được thêm vào KeywordTeacher table
        /// để xác định xem giảng viên dạy môn nào.</param>
        /// <returns>TeacherModel</returns>
        Task<TeacherModel> Crawl(string url, string courseId);

        Task<string> OnDownloadImage(string teacherId, string folder);
    }
}
