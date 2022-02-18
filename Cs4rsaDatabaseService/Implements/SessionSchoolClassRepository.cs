using Cs4rsaDatabaseService.DataProviders;
using Cs4rsaDatabaseService.Interfaces;
using Cs4rsaDatabaseService.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Cs4rsaDatabaseService.Implements
{
    internal class SessionSchoolClassRepository : GenericRepository<SessionSchoolClass>, ISessionSchoolClassRepository
    {
        public SessionSchoolClassRepository(Cs4rsaDbContext context) : base(context)
        {

        }
        public IEnumerable<SessionSchoolClass> GetSessionSchoolClass(SessionDetail sessionDetail)
        {
            SessionDetail querySessionDetail = _context.SessionDetails
                .Where(sd => sd.SessionDetailId == sessionDetail.SessionDetailId)
                .Include(sd => sd.SessionSchoolClasses)
                .FirstOrDefault();
            return querySessionDetail.SessionSchoolClasses.AsEnumerable();
        }
    }
}
