using Cs4rsaDatabaseService.Models;
using System.Threading.Tasks;

namespace Cs4rsaDatabaseService.Interfaces
{
    public interface IStudentRepository: IGenericRepository<Student>
    {
        Task<Student> GetByStudentIdAsync(string id);
    }
}
