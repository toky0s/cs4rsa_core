using Cs4rsa.Service.Conflict.Models;
using Cs4rsa.Service.SubjectCrawler.DataTypes.Enums;

namespace Cs4rsa.Service.Conflict.DataTypes
{
    /// <summary>
    /// Mọi Conflict đều phải kế thừa từ lớp này
    /// </summary>
    public abstract class BaseConflict
    {
        public Lesson LessonA { get; }
        public Lesson LessonB { get; }
        private Phase _phase;
        public Phase Phase => _phase == default ? _phase = GetPhase() : _phase;

        public BaseConflict(Lesson lessonA, Lesson lessonB)
        {
            LessonA = lessonA;
            LessonB = lessonB;
        }

        protected Phase GetPhase()
        {
            if (LessonA.Phase == Phase.First && LessonB.Phase == Phase.First ||
                    LessonA.Phase == Phase.First && LessonB.Phase == Phase.All ||
                    LessonA.Phase == Phase.All && LessonB.Phase == Phase.First)
                return Phase.First;
            if (LessonA.Phase == Phase.Second && LessonB.Phase == Phase.Second ||
                    LessonA.Phase == Phase.Second && LessonB.Phase == Phase.All ||
                    LessonA.Phase == Phase.All && LessonB.Phase == Phase.Second)
                return Phase.Second;
            return Phase.All;
        }
    }
}
