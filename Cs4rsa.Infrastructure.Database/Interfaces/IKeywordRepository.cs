using System.Collections.Generic;
using Cs4rsa.Database.Models;

namespace Cs4rsa.Database.Interfaces
{
    public interface IKeywordRepository
    {
        /// <summary>
        /// Cập nhật lại HTML Cache
        /// </summary>
        /// <param name="keywordId">Keyword Id</param>
        /// <param name="htmlCache">HTML</param>
        /// <returns>Số lượng record đã cập nhật. Nếu trả về 1 cập nhật thành công, ngược lại trả về 0.</returns>
        int UpdateCacheByKeywordId(int keywordId, string htmlCache);
        Keyword GetKeyword(string discipline, string keyword1);

        /// <summary>
        /// GetByPaging Keyword bằng Course ID
        /// </summary>
        /// <param name="courseId">Course ID</param>
        Keyword GetKeyword(string courseId);

        /// <summary>
        /// GetByPaging Keyword bằng Subject Code
        /// </summary>
        /// <param name="subjectCode">Mã môn</param>
        /// <returns></returns>
        Keyword GetKeywordBySubjectCode(string subjectCode);
        string GetColorWithSubjectCode(string subjectCode);
        string GetColor(string courseId);
        /// <summary>
        /// Lấy ra Color dựa theo mã môn.
        /// </summary>
        /// <param name="subjectCode">Mã môn</param>
        /// <returns>Color</returns>
        string GetColorBySubjectCode(string subjectCode);
        long Count(string discipline, string keyword1);

        /// <summary>
        /// Lấy ra cache dựa theo Course ID.
        /// </summary>
        /// <remarks>
        /// Sử dụng RawSql.
        /// </remarks>
        /// <param name="courseId">Course ID</param>
        /// <returns>Cache</returns>
        string GetCache(string courseId);
        string GetCache(string discipline, string keyword1);
        /// <summary>
        /// Lấy ra danh sách các Keyword dựa theo Discipline ID.
        /// </summary>
        /// <remarks>Sử dụng RawSql, không Early Load Discipline.</remarks>
        /// <param name="disciplineId">Discipline ID</param>
        /// <returns>Danh sách các Keyword.</returns>
        List<Keyword> GetKeywordsByDisciplineId(int disciplineId);
        int Insert(Keyword keyword);
        int DeleteAll();
        Keyword GetByCourseId(string courseId);
        long Count();
    }
}
