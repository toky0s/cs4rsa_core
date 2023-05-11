using Cs4rsa.Cs4rsaDatabase.Models;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cs4rsa.Cs4rsaDatabase.Interfaces
{
    public interface IDisciplineRepository : IGenericRepository<Discipline>
    {
        IEnumerable<Discipline> GetAllIncludeKeyword();
        /// <summary>
        /// Lấy ra tất cả Discipline.
        /// </summary>
        /// <remarks>
        /// RawlSql và Không Early Load Keywords.
        /// </remarks>
        /// <returns>Danh sách Discipline</returns>
        IEnumerable<Discipline> GetAllDiscipline();
        Discipline GetDisciplineByID(int id);
    }
}
