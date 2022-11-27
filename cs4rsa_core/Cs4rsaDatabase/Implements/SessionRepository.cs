using Cs4rsa.Cs4rsaDatabase.DataProviders;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;

using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Linq;

namespace Cs4rsa.Cs4rsaDatabase.Implements
{
    public class SessionRepository : GenericRepository<UserSchedule>, IUserScheduleRepository
    {
        public SessionRepository(Cs4rsaDbContext context) : base(context)
        {
        }

        //public async Task<List<SessionDetail>> GetSessionDetails(int sessionId)
        //{
        //    return await _context.SessionDetails
        //        .Where(sessionDetail => sessionDetail.UserScheduleId == sessionId)
        //        .AsNoTracking()
        //        .ToListAsync();
        //}

        IEnumerable<ScheduleDetail> IUserScheduleRepository.GetSessionDetails(int sessionId)
        {
            return _context.SessionDetails
                .Where(sessionDetail => sessionDetail.UserScheduleId == sessionId)
                .AsNoTracking();
        }
    }
}
