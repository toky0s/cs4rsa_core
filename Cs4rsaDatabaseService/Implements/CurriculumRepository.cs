using Cs4rsaDatabaseService.DataProviders;
using Cs4rsaDatabaseService.Interfaces;
using Cs4rsaDatabaseService.Models;

namespace Cs4rsaDatabaseService.Implements
{
    public class CurriculumRepository : GenericRepository<Curriculum>, ICurriculumRepository
    {
        public CurriculumRepository(Cs4rsaDbContext context) : base(context)
        {
        }
    }
}
