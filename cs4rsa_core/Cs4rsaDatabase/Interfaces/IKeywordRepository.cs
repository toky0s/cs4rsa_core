using cs4rsa_core.Cs4rsaDatabase.Models;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace cs4rsa_core.Cs4rsaDatabase.Interfaces
{
    public interface IKeywordRepository : IGenericRepository<Keyword>
    {
        int GetCourseId(string subjectCode);
        Task<Keyword> GetKeyword(string discipline, string keyword1);
        Task<Keyword> GetKeyword(int courseId);
        Task<Keyword> GetKeyword(string subjectCode);
        string GetColorWithSubjectCode(string subjectCode);
        Task<string> GetColorAsync(int courseId);
        bool IsHasColor(string color);
        Task<int> CountAsync(string discipline, string keyword);
        Task<List<Keyword>> GetBySubjectNameContains(string subjectName);
        Task<List<Keyword>> GetByDisciplineAndKeyword1(string discipline, string keyword);
        Task<List<Keyword>> GetByDisciplineStartWith(string text);
        Task<string> GetSubjectCode(int courseId);
    }
}
