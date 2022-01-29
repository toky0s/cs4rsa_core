using Cs4rsaDatabaseService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cs4rsaDatabaseService.Interfaces
{
    public interface ISessionRepository : IGenericRepository<Session>
    {
        IEnumerable<SessionDetail> GetSessionDetails(int sessionId);
    }
}
