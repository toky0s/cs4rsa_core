using Cs4rsa.Cs4rsaDatabase.Models;

namespace Cs4rsa.Cs4rsaDatabase.Interfaces
{
    public interface IKeywordTeacherRepository
    {
        bool ExistByTeacherIdAndCourseId(int teacherId, int courseId);
        void Add(KeywordTeacher kt);
        bool Exists(int teacherId, int courseId);
    }
}
