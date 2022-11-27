using Cs4rsa.Cs4rsaDatabase.Models;

using System.Collections.Generic;

namespace Cs4rsa.Cs4rsaDatabase.Interfaces
{
    public interface ITeacherRepository : IGenericRepository<Teacher>
    {
        IAsyncEnumerable<Teacher> GetTeachersAsync(int page, int limit);
        IEnumerable<Teacher> GetTeacherByNameOrId(string nameOrId);
    }
}
