using System.Collections.Generic;
using Cs4rsa.Database.Models;

namespace Cs4rsa.Database.Interfaces
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
