using Cs4rsaDatabaseService.DataProviders;
using Cs4rsaDatabaseService.Interfaces;
using Cs4rsaDatabaseService.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

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
