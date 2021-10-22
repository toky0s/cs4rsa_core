using Cs4rsaDatabaseService.DataProviders;
using Cs4rsaDatabaseService.Interfaces;
using Cs4rsaDatabaseService.Models;

namespace Cs4rsaDatabaseService.Implements
{
    public class DisciplineRepository : GenericRepository<Discipline>, IDisciplineRepository
    {
        public DisciplineRepository(Cs4rsaDbContext context) : base(context)
        {

        }
    }
}
