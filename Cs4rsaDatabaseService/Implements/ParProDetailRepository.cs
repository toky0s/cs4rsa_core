using Cs4rsaDatabaseService.DataProviders;
using Cs4rsaDatabaseService.Interfaces;
using Cs4rsaDatabaseService.Models;

namespace Cs4rsaDatabaseService.Implements
{
    public class ParProDetailRepository : GenericRepository<ParProDetail>, IParProDetailsRepository
    {
        public ParProDetailRepository(Cs4rsaDbContext context) : base(context)
        {
        }
    }
}
