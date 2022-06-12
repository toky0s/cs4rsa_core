using Cs4rsaDatabaseService.Models;

using System.Collections.Generic;

namespace Cs4rsaDatabaseService.Interfaces
{
    public interface ISessionSchoolClassRepository : IGenericRepository<SessionSchoolClass>
    {
        IEnumerable<SessionSchoolClass> GetSessionSchoolClass(SessionDetail sessionDetail);
    }
}
