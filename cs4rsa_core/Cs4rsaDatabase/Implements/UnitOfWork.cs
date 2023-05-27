using Cs4rsa.Cs4rsaDatabase.Interfaces;

namespace Cs4rsa.Cs4rsaDatabase.Implements
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork()
        {
            Curriculums = new CurriculumRepository();
            Disciplines = new DisciplineRepository();
            Keywords = new KeywordRepository();
            UserSchedules = new SessionRepository();
            Students = new StudentRepository();
            Teachers = new TeacherRepository();
            ProgramSubjects = new ProgramSubjectRepository();
            KeywordTeachers = new KeywordTeacherRepository();
            Settings = new SettingRepository();
        }

        public ICurriculumRepository Curriculums { get; private set; }
        public IDisciplineRepository Disciplines { get; private set; }
        public IKeywordRepository Keywords { get; private set; }
        public IUserScheduleRepository UserSchedules { get; private set; }
        public IStudentRepository Students { get; private set; }
        public ITeacherRepository Teachers { get; private set; }
        public IProgramSubjectRepository ProgramSubjects { get; private set; }
        public IKeywordTeacherRepository KeywordTeachers { get; private set; }
        public ISettingRepository Settings { get; private set; }
    }
}
