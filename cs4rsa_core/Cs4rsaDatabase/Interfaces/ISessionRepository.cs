using cs4rsa_core.Cs4rsaDatabase.Models;

using System.Collections.Generic;

namespace cs4rsa_core.Cs4rsaDatabase.Interfaces
{
    public interface ISessionRepository : IGenericRepository<Session>
    {
        IEnumerable<SessionDetail> GetSessionDetails(int sessionId);
    }
}
