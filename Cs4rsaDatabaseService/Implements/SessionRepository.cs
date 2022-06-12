using Cs4rsaDatabaseService.DataProviders;
using Cs4rsaDatabaseService.Interfaces;
using Cs4rsaDatabaseService.Models;

using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Linq;

namespace Cs4rsaDatabaseService.Implements
{
    public class SessionRepository : GenericRepository<Session>, ISessionRepository
    {
        public SessionRepository(Cs4rsaDbContext context) : base(context)
        {
        }

        //public async Task<List<SessionDetail>> GetSessionDetails(int sessionId)
        //{
        //    return await _context.SessionDetails
        //        .Where(sessionDetail => sessionDetail.SessionId == sessionId)
        //        .AsNoTracking()
        //        .ToListAsync();
        //}

        IEnumerable<SessionDetail> ISessionRepository.GetSessionDetails(int sessionId)
        {
            return _context.SessionDetails
                .Where(sessionDetail => sessionDetail.SessionId == sessionId)
                .AsNoTracking();
        }
    }
}
