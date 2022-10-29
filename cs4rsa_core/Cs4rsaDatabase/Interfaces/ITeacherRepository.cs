using cs4rsa_core.Cs4rsaDatabase.Models;

using System.Collections.Generic;

namespace cs4rsa_core.Cs4rsaDatabase.Interfaces
{
    public interface ITeacherRepository : IGenericRepository<Teacher>
    {
        IAsyncEnumerable<Teacher> GetTeachersAsync(int page, int limit);
        IEnumerable<Teacher> GetTeacherByNameOrId(string nameOrId);
    }
}
