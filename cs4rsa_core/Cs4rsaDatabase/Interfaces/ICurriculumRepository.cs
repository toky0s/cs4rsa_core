using Cs4rsa.Cs4rsaDatabase.Models;

using System.Collections.Generic;

namespace Cs4rsa.Cs4rsaDatabase.Interfaces
{
    public interface ICurriculumRepository : IGenericRepository<Curriculum>
    {
        IEnumerable<Curriculum> GetAllCurr();
        int GetCountMajorSubjectByCurrId(int currId);
        bool ExistsById(string currId);
    }
}
