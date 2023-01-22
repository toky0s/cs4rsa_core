using Cs4rsa.Cs4rsaDatabase.Models;

namespace Cs4rsa.Cs4rsaDatabase.Interfaces
{
    public interface IPreParSubjectRepository : IGenericRepository<DbPreParSubject>
    {
        public DbPreParSubject GetById(int id);
    }
}
