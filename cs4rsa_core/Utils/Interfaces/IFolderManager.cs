namespace Cs4rsa.Utils.Interfaces
{
    /// <summary>
    /// Class triển khai Interface này sẽ triển khai
    /// các phương thức thao tác với thư mục.
    /// </summary>
    public interface IFolderManager
    {
        public static readonly string FD_STUDENT_PROGRAMS = "StudentPrograms";
        public static readonly string FD_STUDENT_PLANS = "StudentPlans";
        public static readonly string FD_TEACHER_IMAGES = "TeacherImages";
        string CreateFolderIfNotExists(string path);
    }
}
