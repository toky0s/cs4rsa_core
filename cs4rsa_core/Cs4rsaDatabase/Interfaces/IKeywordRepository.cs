using Cs4rsa.Cs4rsaDatabase.Models;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cs4rsa.Cs4rsaDatabase.Interfaces
{
    public interface IKeywordRepository : IGenericRepository<Keyword>
    {
        int GetCourseId(string subjectCode);
        Task<Keyword> GetKeyword(string discipline, string keyword1);

        /// <summary>
        /// Get Keyword bằng Course ID
        /// </summary>
        /// <param name="courseId">Course ID</param>
        Task<Keyword> GetKeyword(int courseId);
        Task<Keyword> GetKeyword(string subjectCode);
        string GetColorWithSubjectCode(string subjectCode);
        Task<string> GetColorAsync(int courseId);
        bool IsHasColor(string color);
        Task<int> CountAsync(string discipline, string keyword);
        IAsyncEnumerable<Keyword> GetBySubjectNameContains(string subjectName);
        IAsyncEnumerable<Keyword> GetByDisciplineAndKeyword1(string discipline, string keyword);
        IAsyncEnumerable<Keyword> GetByDisciplineStartWith(string text);
        Task<string> GetSubjectCode(int courseId);
    }
}
