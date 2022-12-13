using Cs4rsa.Cs4rsaDatabase.DataProviders;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;

namespace Cs4rsa.Cs4rsaDatabase.Implements
{
    public class PreParSubjectRepository : GenericRepository<DbPreParSubject>, IPreParSubjectRepository
    {
        public PreParSubjectRepository(Cs4rsaDbContext context) : base(context)
        {
        }

        public DbPreParSubject GetById(int id)
        {
            return _context.PreParSubjects.Find(id);
        }
    }
}
