/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using CwebizAPI.Share.Database.Models;

namespace CwebizAPI.Db.Interfaces
{
    /// <summary>
    /// Interface Student Repository
    /// </summary>
    /// <remarks>
    /// Created Date: 21/06/2023
    /// Modified Date: 21/06/2023
    /// Author: Truong A Xin
    /// </remarks>
    public interface IStudentRepository
    {
        #region Curriculum
        IEnumerable<Curriculum> GetAllCurr();
        int GetCountMajorSubjectByCurrId(int currId);
        bool CurriculumExistsById(int currId);
        Curriculum GetCurriculumById(int currId);
        
        /// <summary>
        /// Thêm mới một Curriculum.
        /// </summary>
        /// <param name="curriculum">Curriculum</param>
        void Insert(Curriculum curriculum);
        #endregion

        #region Student
        /// <summary>
        /// Thêm mới một sinh viên.
        /// </summary>
        /// <param name="student">Student</param>
        void Insert(Student student);
        
        /// <summary>
        /// Lấy thông tin sinh viên bằng mã sinh viên.
        /// </summary>
        /// <param name="id">Mã sinh viên.</param>
        /// <returns>Student</returns>
        Task<Student> GetByStudentId(string id);
        bool ExistsBySpecialString(string specialString);
        
        /// <summary>
        /// Kiểm tra tồn tại bằng mã sinh viên.
        /// </summary>
        /// <param name="studentCode">Mã sinh viên</param>
        /// <returns>bool</returns>
        Task<bool> ExistsByStudentCode(string studentCode);
        
        /// <summary>
        /// Lấy ra danh sách sinh viên có chứa mã sinh viên được chỉ định.
        /// </summary>
        /// <param name="studentId">Mã sinh viên.</param>
        /// <param name="limit">Số phần tử mỗi trang.</param>
        /// <param name="page">Số trang.</param>
        /// <returns>Danh sách sinh viên.</returns>
        IEnumerable<Student> GetStudentsByContainsId(string studentId, int limit, int page);
        
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
        IEnumerable<Student> GetAllBySpecialStringNotNull();
        int Remove(Student student);
        int Update(Student student);
        void Add(Student student);
        Student GetBySpecialString(string specialString);
        IEnumerable<Student> GetAll();
        
        /// <summary>
        /// Kiểm tra Student đã link với bất kỳ tài khoản nào hay chưa.
        /// </summary>
        /// <param name="studentId">Mã sinh viên</param>
        /// <returns>True nếu student đã được liên kết, ngược lại trả về False.</returns>
        Task<bool> IsLinked(string studentId);
        #endregion
    }
}
