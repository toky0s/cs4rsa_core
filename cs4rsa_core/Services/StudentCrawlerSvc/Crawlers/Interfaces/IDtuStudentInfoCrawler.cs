using Cs4rsa.Cs4rsaDatabase.Models;

using System.Threading.Tasks;

namespace Cs4rsa.Services.StudentCrawlerSvc.Crawlers.Interfaces
{
    public interface IDtuStudentInfoCrawler
    {
        Task<Student> Crawl(string specialString);

        /// <summary>
        /// Tải hình ảnh hồ sơ sinh viên.
        /// </summary>
        /// <param name="studentCode">Mã sinh viên</param>
        /// <returns>
        /// Nếu thành công trả về một string là mã sinh viên, 
        /// ngược lại nếu gặp lỗi sẽ trả về null.
        /// </returns>
        Task<string> DownloadProfileImg(string studentCode);
    }
}
