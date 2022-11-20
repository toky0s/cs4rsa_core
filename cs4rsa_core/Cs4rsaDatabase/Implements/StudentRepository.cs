using Cs4rsa.Cs4rsaDatabase.DataProviders;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;

using System.Threading.Tasks;

namespace Cs4rsa.Cs4rsaDatabase.Implements
{
    public class StudentRepository : GenericRepository<Student>, IStudentRepository
    {
        public StudentRepository(Cs4rsaDbContext context) : base(context)
        {
        }

        public async Task<Student> GetByStudentIdAsync(string id)
        {
            return await _context.Set<Student>().FindAsync(id);
        }
    }
}
