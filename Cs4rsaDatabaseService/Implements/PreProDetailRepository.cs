using Cs4rsaDatabaseService.DataProviders;
using Cs4rsaDatabaseService.Interfaces;
using Cs4rsaDatabaseService.Models;

namespace Cs4rsaDatabaseService.Implements
{
    public class PreProDetailRepository : GenericRepository<PreProDetail>, IPreProDetailsRepository
    {
        public PreProDetailRepository(Cs4rsaDbContext context) : base(context)
        {
        }
    }
}
