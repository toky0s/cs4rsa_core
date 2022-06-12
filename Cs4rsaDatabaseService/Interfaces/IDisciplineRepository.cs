using Cs4rsaDatabaseService.Models;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cs4rsaDatabaseService.Interfaces
{
    public interface IDisciplineRepository : IGenericRepository<Discipline>
    {
        Task<List<Discipline>> GetAllIncludeKeywordAsync();
    }
}
