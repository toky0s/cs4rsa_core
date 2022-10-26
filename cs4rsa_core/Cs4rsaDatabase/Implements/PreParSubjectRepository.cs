using cs4rsa_core.Cs4rsaDatabase.DataProviders;
using cs4rsa_core.Cs4rsaDatabase.Interfaces;
using cs4rsa_core.Cs4rsaDatabase.Models;

namespace cs4rsa_core.Cs4rsaDatabase.Implements
{
    public class PreParSubjectRepository : GenericRepository<PreParSubject>, IPreParSubjectRepository
    {
        public PreParSubjectRepository(Cs4rsaDbContext context) : base(context)
        {
        }

        public PreParSubject GetById(int id)
        {
            return _context.PreParSubjects.Find(id);
        }
    }
}
