using Cs4rsa.Cs4rsaDatabase.Models;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cs4rsa.Cs4rsaDatabase.Interfaces
{
    public interface IKeywordRepository : IGenericRepository<Keyword>
    {
        Task<Keyword> GetKeyword(string discipline, string keyword1);

        /// <summary>
        /// Get Keyword bằng Course ID
        /// </summary>
        /// <param name="courseId">Course ID</param>
        Task<Keyword> GetKeyword(int courseId);

        /// <summary>
        /// Get Keyword bằng Subject Code
        /// </summary>
        /// <param name="subjectCode">Mã môn</param>
        /// <returns></returns>
        Task<Keyword> GetKeyword(string subjectCode);
        string GetColorWithSubjectCode(string subjectCode);
        Task<string> GetColorAsync(int courseId);
        Task<int> CountAsync(string discipline, string keyword);
        IAsyncEnumerable<Keyword> GetBySubjectNameContains(string subjectName);
        IAsyncEnumerable<Keyword> GetByDisciplineAndKeyword1(string discipline, string keyword);
        IAsyncEnumerable<Keyword> GetByDisciplineStartWith(string text);

        /// <summary>
        /// Kiểm tra tồn tại bằng Subject Code
        /// </summary>
        /// <param name="subjectCode">Mã môn</param>
        /// <returns>Tồn tại trả về true, ngược lại trả về false.</returns>
        Task<bool> ExistBySubjectCodeAsync(string subjectCode);
    }
}
