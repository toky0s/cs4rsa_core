using Cs4rsaDatabaseService.DataProviders;
using Cs4rsaDatabaseService.Interfaces;
using Cs4rsaDatabaseService.Models;

namespace Cs4rsaDatabaseService.Implements
{
    public class PreParSubjectRepository : GenericRepository<PreParSubject>, IPreParSubjectRepository
    {
        public PreParSubjectRepository(Cs4rsaDbContext context) : base(context)
        {
        }
    }
}
