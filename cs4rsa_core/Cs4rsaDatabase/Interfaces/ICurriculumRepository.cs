using Cs4rsa.Cs4rsaDatabase.Models;

using System.Collections.Generic;

namespace Cs4rsa.Cs4rsaDatabase.Interfaces
{
    public interface ICurriculumRepository
    {
        List<Curriculum> GetAllCurr();
        int GetCountMajorSubjectByCurrId(int currId);
        bool ExistsById(int currId);
        Curriculum GetByID(int currId);
        int Insert(Curriculum curriculum);
    }
}
