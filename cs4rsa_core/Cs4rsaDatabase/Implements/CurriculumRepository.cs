using Cs4rsa.Cs4rsaDatabase.DataProviders;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;

namespace Cs4rsa.Cs4rsaDatabase.Implements
{
    public class CurriculumRepository : GenericRepository<Curriculum>, ICurriculumRepository
    {
        public CurriculumRepository(Cs4rsaDbContext context, RawSql rawSql) : base(context, rawSql)
        {
        }
    }
}
