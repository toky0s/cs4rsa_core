using cs4rsa_core.Cs4rsaDatabase.DataProviders;
using cs4rsa_core.Cs4rsaDatabase.Interfaces;
using cs4rsa_core.Cs4rsaDatabase.Models;

using System.Threading.Tasks;

namespace cs4rsa_core.Cs4rsaDatabase.Implements
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
