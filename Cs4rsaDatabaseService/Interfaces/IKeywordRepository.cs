using Cs4rsaDatabaseService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        string GetColor(int courseId);
        bool IsHasColor(string color);
    }
}
