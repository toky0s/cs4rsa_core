using Cs4rsa.Cs4rsaDatabase.Models;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cs4rsa.Cs4rsaDatabase.Interfaces
{
    public interface IStudentRepository : IGenericRepository<Student>
    {
        Task<Student> GetByStudentIdAsync(string id);
        Task<Student> GetBySpecialStringAsync(string specialString);
        Task<bool> ExistsBySpecialString(string specialString);
        /// <summary>
        /// Lấy ra danh sách sinh viên có chứa mã sinh viên được chỉ định.
        /// </summary>
        /// <param name="studentId">Mã sinh viên.</param>
        /// <param name="limit">Số phần tử mỗi trang.</param>
        /// <param name="page">Số trang.</param>
        /// <returns>Danh sách sinh viên.</returns>
        IAsyncEnumerable<Student> GetStudentsByContainsId(string studentId, int limit, int page);
        /// <summary>
        /// Đếm số lượng sinh viên trong mã sinh viên có chứa.
        /// </summary>
        /// <param name="studentId">Mã sinh viên.</param>
        /// <returns>Số lượng khớp.</returns>
        Task<int> CountByContainsId(string studentId);
    }
}
