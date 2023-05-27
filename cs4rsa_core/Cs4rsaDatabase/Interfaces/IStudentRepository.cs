using Cs4rsa.Cs4rsaDatabase.Models;

using System.Collections.Generic;

namespace Cs4rsa.Cs4rsaDatabase.Interfaces
{
    public interface IStudentRepository
    {
        Student GetByStudentId(string id);
        bool ExistsBySpecialString(string specialString);
        /// <summary>
        /// Lấy ra danh sách sinh viên có chứa mã sinh viên được chỉ định.
        /// </summary>
        /// <param name="studentId">Mã sinh viên.</param>
        /// <param name="limit">Số phần tử mỗi trang.</param>
        /// <param name="page">Số trang.</param>
        /// <returns>Danh sách sinh viên.</returns>
        List<Student> GetStudentsByContainsId(string studentId, int limit, int page);
        List<Student> GetByPaging(int limit, int page);
        /// <summary>
        /// Đếm số lượng sinh viên trong mã sinh viên có chứa.
        /// </summary>
        /// <param name="studentId">Mã sinh viên.</param>
        /// <returns>Số lượng khớp.</returns>
        long CountByContainsId(string studentId);
        /// <summary>
        /// Lấy ra tất cả các sinh viên có SpecialString.
        /// </summary>
        /// <returns>Danh sách sinh viên.</returns>
        List<Student> GetAllBySpecialStringNotNull();
        int Remove(Student student);
        int Update(Student student);
        void Add(Student student);
        long CountPage(int limit);
        Student GetBySpecialString(string specialString);
    }
}
