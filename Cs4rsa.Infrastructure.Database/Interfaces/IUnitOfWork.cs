namespace Cs4rsa.Database.Interfaces
{
    public interface IUnitOfWork
    {
        ICurriculumRepository Curriculums { get; }
        IDisciplineRepository Disciplines { get; }
        IKeywordRepository Keywords { get; }
        IUserScheduleRepository UserSchedules { get; }
        IStudentRepository Students { get; }
        ITeacherRepository Teachers { get; }
        IProgramSubjectRepository ProgramSubjects { get; }
        IKeywordTeacherRepository KeywordTeachers { get; }
        ISettingRepository Settings { get; }
    }
}
