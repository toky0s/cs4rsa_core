using CwebizAPI.Crawlers.TeacherCrawlerSvc.Models;

namespace CwebizAPI.Crawlers.TeacherCrawlerSvc.Crawlers.Interfaces
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
        Task<TeacherModel> Crawl(string url, int courseId);
    }
}
