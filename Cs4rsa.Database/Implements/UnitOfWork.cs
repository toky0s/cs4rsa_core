using Cs4rsa.Database.DataProviders;
using Cs4rsa.Database.Interfaces;

namespace Cs4rsa.Database.Implements
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(RawSql rawSql)
        {
            Curriculums = new CurriculumRepository(rawSql);
            Disciplines = new DisciplineRepository(rawSql);
            Keywords = new KeywordRepository(rawSql);
            UserSchedules = new SessionRepository(rawSql);
            Students = new StudentRepository(rawSql);
            Teachers = new TeacherRepository(rawSql);
            ProgramSubjects = new ProgramSubjectRepository(rawSql);
            KeywordTeachers = new KeywordTeacherRepository(rawSql);
            Settings = new SettingRepository(rawSql);
        }
        
        public ICurriculumRepository Curriculums { get; }
        public IDisciplineRepository Disciplines { get; }
        public IKeywordRepository Keywords { get; }
        public IUserScheduleRepository UserSchedules { get; }
        public IStudentRepository Students { get; }
        public ITeacherRepository Teachers { get; }
        public IProgramSubjectRepository ProgramSubjects { get; }
        public IKeywordTeacherRepository KeywordTeachers { get; }
        public ISettingRepository Settings { get; }
    }
}
