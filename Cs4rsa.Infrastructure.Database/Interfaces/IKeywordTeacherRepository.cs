using Cs4rsa.Database.Models;

namespace Cs4rsa.Database.Interfaces
{
    public interface IKeywordTeacherRepository
    {
        bool ExistByTeacherIdAndCourseId(int teacherId, int courseId);
        void Add(KeywordTeacher kt);
        bool Exists(int teacherId, int courseId);
    }
}
