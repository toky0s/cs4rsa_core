using cs4rsa_core.Cs4rsaDatabase.Models;

namespace cs4rsa_core.Cs4rsaDatabase.Interfaces
{
    public interface IPreParSubjectRepository : IGenericRepository<PreParSubject>
    {
        public PreParSubject GetById(int id);
    }
}
