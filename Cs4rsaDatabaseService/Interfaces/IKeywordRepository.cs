using Cs4rsaDatabaseService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cs4rsaDatabaseService.Interfaces
{
    public interface IKeywordRepository : IGenericRepository<Keyword>
    {
        int GetCourseId(string subjectCode);
        Keyword GetKeyword(string discipline, string keyword1);
        Keyword GetKeyword(int courseId);
        Keyword GetKeyword(string subjectCode);
        string GetColorWithSubjectCode(string subjectCode);
        Task<string> GetColorAsync(int courseId);
        bool IsHasColor(string color);
        Task<int> CountAsync(string discipline, string keyword);
        Task<List<Keyword>> GetBySubjectNameContains(string subjectName);
        Task<List<Keyword>> GetByDisciplineAndKeyword1(string discipline, string keyword);
        Task<List<Keyword>> GetByDisciplineStartWith(string text);
    }
}
