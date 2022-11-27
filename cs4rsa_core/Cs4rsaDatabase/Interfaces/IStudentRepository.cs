using Cs4rsa.Cs4rsaDatabase.Models;

using System.Threading.Tasks;

namespace Cs4rsa.Cs4rsaDatabase.Interfaces
{
    public interface IStudentRepository : IGenericRepository<Student>
    {
        Task<Student> GetByStudentIdAsync(string id);
        Task<Student> GetBySpecialStringAsync(string specialString);
    }
}
