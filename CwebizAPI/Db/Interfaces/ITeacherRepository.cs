using CwebizAPI.Share.Database.Models;

namespace CwebizAPI.Db.Interfaces;

public interface ITeacherRepository
{
    /// <summary>
    /// Lấy tất cả các giảng viên trong DB.
    /// </summary>
    /// <returns>Danh sách các giảng viên.</returns>
    List<Teacher> GetTeachers();
    List<Teacher> GetTeachers(int page, int limit);
    List<Teacher> GetTeacherByNameOrId(string nameOrId);
    Teacher GetTeacherByName(string name);
    Teacher GetTeacherById(int id);
    bool ExistById(int id);
    int Update(Teacher teacher);
    void Add(Teacher teacher);
    long CountPage(int size);
}