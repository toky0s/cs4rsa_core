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
        public static readonly string FD_STUDENT_IMGS = "StudentImages";
        public static readonly string FD_TEACHER_IMAGES = "TeacherImages";
        public static readonly string FD_HTML_CACHES = "HtmlCaches";

        public static readonly string[] FOLDERS = new string[] {
              FD_STUDENT_PROGRAMS
            , FD_STUDENT_PLANS
            , FD_STUDENT_IMGS
            , FD_TEACHER_IMAGES
            , FD_HTML_CACHES
        };

        string CreateFolderIfNotExists(string path);
        void CreateFoldersAtStartUp();

        /// <summary>
        /// Xoá bỏ mọi file có trong folder.
        /// </summary>
        /// <param name="folderPath">Folder Path</param>
        void DelAllInThisFolder(string folderPath);
    }
}
