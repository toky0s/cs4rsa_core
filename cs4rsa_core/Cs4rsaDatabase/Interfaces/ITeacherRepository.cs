using Cs4rsa.Cs4rsaDatabase.Models;

using System.Collections.Generic;

namespace Cs4rsa.Cs4rsaDatabase.Interfaces
{
    public interface ITeacherRepository
    {
        List<Teacher> GetTeachers(int page, int limit);
        List<Teacher> GetTeacherByNameOrId(string nameOrId);
        Teacher GetTeacherByName(string name);
        Teacher GetTeacherById(int id);
        bool ExistByID(int id);
        int Update(Teacher teacher);
        void Add(Teacher teacher);
        long CountPage(int size);
    }
}
