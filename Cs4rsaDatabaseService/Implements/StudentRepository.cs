using Cs4rsaDatabaseService.DataProviders;
using Cs4rsaDatabaseService.Interfaces;
using Cs4rsaDatabaseService.Models;
using System.Threading.Tasks;

namespace Cs4rsaDatabaseService.Implements
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
