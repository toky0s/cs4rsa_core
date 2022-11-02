using cs4rsa_core.Services.TeacherCrawlerSvc.Models;

using System.Threading.Tasks;

namespace cs4rsa_core.Services.TeacherCrawlerSvc.Crawlers.Interfaces
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
        /// <param name="isUpdate">
        /// Cờ cập nhật, nếu cờ này true sau khi cào thông tin của giảng viên sẽ được cập nhật (nếu có), 
        /// ngược lại thông tin của giảng viên sẽ được thêm vào DB.
        /// </param>
        /// <returns></returns>
        Task<TeacherModel> Crawl(string url, int courseId, bool isUpdate);
    }
}
