/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using CwebizAPI.Share.Database.Models;

namespace CwebizAPI.Db.Interfaces
{
    /// <summary>
    /// Interface DisciplineRepository
    /// </summary>
    /// <remarks>
    /// Created Date: 21/06/2023
    /// Modified Date: 21/06/2023
    /// Author: Truong A Xin
    /// </remarks>
    public interface IDisciplineRepository : IDisposable
    {
        #region Discipline
        IEnumerable<Discipline?> GetAllIncludeKeyword();
        
        /// <summary>
        /// Lấy ra danh sách tất cả các Discipline.
        /// </summary>
        /// <returns>Danh sách Discipline.</returns>
        Task<List<Discipline>> GetAllDiscipline();
        Task<Discipline?> GetDisciplineById(int id);
        
        /// <summary>
        /// Lấy ra Discipline bằng tên,.
        /// </summary>
        /// <param name="name">Tên Discipline.</param>
        /// <returns>Discipline.</returns>
        Task<Discipline> GetDisciplineByName(string? name);
        
        /// <summary>
        /// Xoá tất cả các Discipline và Keyword liên quan.
        /// </summary>
        void DeleteAllDisciplineAndKeyword();
        void Insert(Discipline? discipline);
        
        /// <summary>
        /// Bulk Insert Discipline
        /// </summary>
        /// <param name="disciplines">Danh sách Discipline.</param>
        void InsertAll(IEnumerable<Discipline?> disciplines);
        #endregion

        #region Keyword
        /// <summary>
        /// Bulk Insert Keyword.
        /// </summary>
        /// <param name="keywords">Danh sách Keyword.</param>
        void InsertAll(IEnumerable<Keyword> keywords);
        Task UpdateCacheByKeywordId(int keywordId, string cache);
        Task<Keyword> GetKeyword(string discipline, string keyword1);
        
        /// <summary>
        /// Lấy ra danh sách tất cả các Keyword.
        /// </summary>
        /// <returns>Danh sách các Keyword.</returns>
        Task<List<Keyword>> GetAllKeyword();

        /// <summary>
        /// GetByPaging Keyword bằng Course ID
        /// </summary>
        /// <param name="courseId">Course ID</param>
        Task<Keyword> GetKeyword(int courseId);

        /// <summary>
        /// GetByPaging Keyword bằng Subject Code
        /// </summary>
        /// <param name="subjectCode">Mã môn</param>
        /// <returns></returns>
        Task<Keyword> GetKeyword(string subjectCode);
        
        /// <summary>
        /// Lấy ra mã màu bằng mã môn.
        /// </summary>
        /// <param name="subjectCode">Mã môn.</param>
        /// <returns>Mã màu.</returns>
        Task<string> GetColorWithSubjectCode(string subjectCode);
        
        /// <summary>
        /// Lấy ra mã màu bằng Course ID.
        /// </summary>
        /// <param name="courseId">Course ID.</param>
        /// <returns>Mã màu.</returns>
        Task<string> GetColor(int courseId);
        
        /// <summary>
        /// Lấy ra Color dựa theo mã môn.
        /// </summary>
        /// <param name="subjectCode">Mã môn</param>
        /// <returns>Color</returns>
        Task<string> GetColorBySubjectCode(string subjectCode);
        long Count(string discipline, string keyword1);

        /// <summary>
        /// Lấy ra cache dựa theo Course ID.
        /// </summary>
        /// <remarks>
        /// Sử dụng RawSql.
        /// </remarks>
        /// <param name="courseId">Course ID</param>
        /// <returns>Cache</returns>
        Task<string> GetCache(string courseId);
        /// <summary>
        /// Lấy ra danh sách các Keyword dựa theo Discipline ID
        /// </summary>
        /// <param name="disciplineId">Mã ngành</param>
        /// <returns>Danh sách các Keywords</returns>
        Task<List<Keyword>> GetKeywordsByDisciplineId(int disciplineId);
        void Insert(Keyword keyword);
        Task DeleteAllKeywords();
        Task<Keyword> GetByCourseId(int intCourseId);
        Task<long> Count();
        #endregion

        #region CourseInfors
        /// <summary>
        /// Kiểm tra thông tin year value và semester value
        /// có trùng với giá trị mới nhất hiện có hay không.
        /// </summary>
        /// <param name="yearValue">Year value</param>
        /// <param name="semesterValue">Semester value</param>
        /// <returns></returns>
        Task<bool> Exists(string yearValue, string semesterValue);
        
        /// <summary>
        /// Thêm mới một Course.
        /// </summary>
        /// <param name="course">Course.</param>
        /// <returns>Course đã thêm mới.</returns>
        Course? Insert(Course? course);
        
        /// <summary>
        /// Lấy thông tin Course bằng mã năm và mã kỳ.
        /// </summary>
        /// <param name="yearValue">Mã năm.</param>
        /// <param name="semesterValue">Mã kỳ.</param>
        /// <returns>Course.</returns>
        Task<Course?> GetCourse(string yearValue, string semesterValue);
        #endregion
    }
}
