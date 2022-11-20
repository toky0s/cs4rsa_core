using Cs4rsa.Cs4rsaDatabase.Models;

namespace Cs4rsa.Cs4rsaDatabase.Interfaces
{
    public interface IPreParSubjectRepository : IGenericRepository<PreParSubject>
    {
        public PreParSubject GetById(int id);
    }
}
