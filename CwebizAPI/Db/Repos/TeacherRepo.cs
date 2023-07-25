using CwebizAPI.Db.Interfaces;
using CwebizAPI.Share.Database.Models;

namespace CwebizAPI.Db.Repos;

public class TeacherRepo : ITeacherRepository
{
    private readonly CwebizContext _cwebizContext;
    
    public TeacherRepo(CwebizContext cwebizContext)
    {
        _cwebizContext = cwebizContext;
    }
    
    public List<Teacher> GetTeachers()
    {
        throw new NotImplementedException();
    }

    public List<Teacher> GetTeachers(int page, int limit)
    {
        throw new NotImplementedException();
    }

    public List<Teacher> GetTeacherByNameOrId(string nameOrId)
    {
        throw new NotImplementedException();
    }

    public Teacher GetTeacherByName(string name)
    {
        throw new NotImplementedException();
    }

    public Teacher GetTeacherById(int id)
    {
        throw new NotImplementedException();
    }

    public bool ExistById(int id)
    {
        throw new NotImplementedException();
    }

    public int Update(Teacher teacher)
    {
        throw new NotImplementedException();
    }

    public void Add(Teacher teacher)
    {
        throw new NotImplementedException();
    }

    public long CountPage(int size)
    {
        throw new NotImplementedException();
    }
}