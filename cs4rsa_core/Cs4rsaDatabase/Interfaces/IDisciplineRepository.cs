using Cs4rsa.Cs4rsaDatabase.Models;

using System.Collections.Generic;
using System.Text;

namespace Cs4rsa.Cs4rsaDatabase.Interfaces
{
    public interface IDisciplineRepository
    {
        List<Discipline> GetAllIncludeKeyword();
        /// <summary>
        /// Lấy ra tất cả Discipline.
        /// </summary>
        /// <remarks>
        /// RawlSql và Không Early Load Keywords.
        /// </remarks>
        /// <returns>Danh sách Discipline</returns>
        List<Discipline> GetAllDiscipline();
        Discipline GetDisciplineByID(int id);
        int DeleteAll();
        int Insert(Discipline discipline);
    }
}
