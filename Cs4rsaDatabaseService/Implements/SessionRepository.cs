using Cs4rsaDatabaseService.DataProviders;
using Cs4rsaDatabaseService.Interfaces;
using Cs4rsaDatabaseService.Models;
using System.Collections.Generic;
using System.Linq;

namespace Cs4rsaDatabaseService.Implements
{
    public class SessionRepository : GenericRepository<Session>, ISessionRepository
    {
        public SessionRepository(Cs4rsaDbContext context) : base(context)
        {
        }

        public List<SessionDetail> GetSessionDetails(int sessionId)
        {
            return _context.SessionDetails.Where(sessionDetail => sessionDetail.SessionId == sessionId).ToList();
        }
    }
}
