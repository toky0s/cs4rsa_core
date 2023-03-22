using Cs4rsa.Cs4rsaDatabase.Models;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cs4rsa.Cs4rsaDatabase.Interfaces
{
    public interface IDisciplineRepository : IGenericRepository<Discipline>
    {
        Task<List<Discipline>> GetAllIncludeKeywordAsync();
        /// <summary>
        /// Lấy ra tất cả Discipline.
        /// </summary>
        /// <remarks>
        /// RawlSql và Không Early Load Keywords.
        /// </remarks>
        /// <returns>Danh sách Discipline</returns>
        List<Discipline> GetAllDiscipline();
    }
}
