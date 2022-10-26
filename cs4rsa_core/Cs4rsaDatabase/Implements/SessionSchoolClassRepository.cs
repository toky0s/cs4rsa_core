using cs4rsa_core.Cs4rsaDatabase.DataProviders;
using cs4rsa_core.Cs4rsaDatabase.Interfaces;
using cs4rsa_core.Cs4rsaDatabase.Models;

using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Linq;

namespace cs4rsa_core.Cs4rsaDatabase.Implements
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
