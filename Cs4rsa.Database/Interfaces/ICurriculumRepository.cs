using System.Collections.Generic;
using Cs4rsa.Database.Models;

namespace Cs4rsa.Database.Interfaces
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
