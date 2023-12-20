using Cs4rsa.Service.Conflict.Models;

namespace Cs4rsa.Service.Conflict.DataTypes
{
    /// <summary>
    /// Mọi Conflict đều phải kế thừa từ lớp này
    /// </summary>
    public abstract class BaseConflict
    {
        public Lesson LessonA { get; }
        public Lesson LessonB { get; }

        public BaseConflict(Lesson lessonA, Lesson lessonB)
        {
            LessonA = lessonA;
            LessonB = lessonB;
        }
    }
}
