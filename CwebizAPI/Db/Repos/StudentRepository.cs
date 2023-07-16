/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using Microsoft.EntityFrameworkCore;
using CwebizAPI.Db.Interfaces;
using CwebizAPI.Share.Database.Models;

namespace CwebizAPI.Db.Repos;

/// <summary>
/// Student Repository
/// </summary>
/// <remarks>
/// Created Date: 21/06/2023
/// Modified Date: 21/06/2023
/// Author: Truong A Xin
/// </remarks>
public class StudentRepository : IStudentRepository
{
    private readonly CwebizContext _cwebizContext;
    
    public StudentRepository(CwebizContext cwebizContext)
    {
        _cwebizContext = cwebizContext;
    }
    
    public IEnumerable<Curriculum> GetAllCurr()
    {
        throw new NotImplementedException();
    }

    public int GetCountMajorSubjectByCurrId(int currId)
    {
        throw new NotImplementedException();
    }

    public bool CurriculumExistsById(int currId)
    {
        throw new NotImplementedException();
    }

    public Curriculum GetCurriculumById(int currId)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Thêm mới mã ngành.
    /// </summary>
    /// <param name="curriculum">Thông tin mã ngành.</param>
    public void Insert(Curriculum curriculum)
    {
        _cwebizContext.Curricula!.Add(curriculum);
    }

    /// <summary>
    /// Thêm mới sinh viên.
    /// </summary>
    /// <param name="student">Thông tin sinh viên.</param>
    public void Insert(Student student)
    {
        _cwebizContext.Students!.Add(student);
    }

    /// <summary>
    /// Lấy ra thông tin sinh viên bằng mã sinh viên.
    /// </summary>
    /// <param name="id">Mã sinh viên.</param>
    /// <returns>Thông tin của sinh viên (nếu có).</returns>
    public async Task<Student> GetByStudentId(string id)
    {
        return await (from s in _cwebizContext.Students
            where s.StudentId.Equals(id)
            select s).FirstAsync();
    }

    public bool ExistsBySpecialString(string specialString)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Kiểm tra tồn tại bằng mã sinh viên.
    /// </summary>
    /// <param name="studentCode">Mã sinh viên.</param>
    /// <returns>Trả về True nếu tồn tại, ngược lại trả về False.</returns>
    public async Task<bool> ExistsByStudentCode(string studentCode)
    {
        return await (from student in _cwebizContext.Students
            where student.StudentId.Equals(studentCode)
            select student).AnyAsync();
    }

    public IEnumerable<Student> GetStudentsByContainsId(string studentId, int limit, int page)
    {
        throw new NotImplementedException();
    }

    public long CountByContainsId(string studentId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Student> GetAllBySpecialStringNotNull()
    {
        throw new NotImplementedException();
    }

    public int Remove(Student student)
    {
        throw new NotImplementedException();
    }

    public int Update(Student student)
    {
        throw new NotImplementedException();
    }

    public void Add(Student student)
    {
        throw new NotImplementedException();
    }

    public Student GetBySpecialString(string specialString)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Student> GetAll()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Kiểm tra liên kết.
    /// </summary>
    /// <param name="studentId">Mã sinh viên</param>
    /// <returns>Trả về True nếu đã liên kết, ngược lại trả về False.</returns>
    /// <exception cref="ArgumentNullException">Exception này sẽ được ném ra nếu Mã sinh viên là NULL.</exception>
    public async Task<bool> IsLinked(string studentId)
    {
        if (studentId == null) throw new ArgumentNullException(nameof(studentId));
        return await (from s in _cwebizContext.Students
            from u in _cwebizContext.CwebizUsers
            where s.StudentId.Equals(studentId) && u.StudentId.Equals(studentId)
            select s).AnyAsync();
    }
}