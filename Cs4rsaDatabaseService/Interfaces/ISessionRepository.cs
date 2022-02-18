using Cs4rsaDatabaseService.Models;
using System.Collections.Generic;

namespace Cs4rsaDatabaseService.Interfaces
{
    public interface ISessionRepository : IGenericRepository<Session>
    {
        IEnumerable<SessionDetail> GetSessionDetails(int sessionId);
    }
}
