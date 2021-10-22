using Cs4rsaDatabaseService.DataProviders;
using Cs4rsaDatabaseService.Interfaces;
using Cs4rsaDatabaseService.Models;

namespace Cs4rsaDatabaseService.Implements
{
    public class TeacherRepository : GenericRepository<Teacher>, ITeacherRepository
    {
        public TeacherRepository(Cs4rsaDbContext context) : base(context)
        {
        }
    }
}
