using Cs4rsaDatabaseService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cs4rsaDatabaseService.Interfaces
{
    public interface ISessionSchoolClassRepository: IGenericRepository<SessionSchoolClass>
    {
        IEnumerable<SessionSchoolClass> GetSessionSchoolClass(SessionDetail sessionDetail);
    }
}
