using cs4rsa_core.Cs4rsaDatabase.DataProviders;
using cs4rsa_core.Cs4rsaDatabase.Interfaces;
using cs4rsa_core.Cs4rsaDatabase.Models;

namespace cs4rsa_core.Cs4rsaDatabase.Implements
{
    public class CurriculumRepository : GenericRepository<Curriculum>, ICurriculumRepository
    {
        public CurriculumRepository(Cs4rsaDbContext context) : base(context)
        {
        }
    }
}
