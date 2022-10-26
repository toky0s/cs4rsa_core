using cs4rsa_core.Cs4rsaDatabase.Models;

using System.Threading.Tasks;

namespace cs4rsa_core.Cs4rsaDatabase.Interfaces
{
    public interface IStudentRepository : IGenericRepository<Student>
    {
        Task<Student> GetByStudentIdAsync(string id);
    }
}
