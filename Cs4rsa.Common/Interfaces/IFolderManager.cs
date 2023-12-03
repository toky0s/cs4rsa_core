namespace Cs4rsa.Common.Interfaces
{
    /// <summary>
    /// Class triển khai Interface này sẽ triển khai
    /// các phương thức thao tác với thư mục.
    /// </summary>
    public abstract class IFolderManager
    {
        public static readonly string FdStudentPrograms = "StudentPrograms";
        public static readonly string FdStudentPlans = "StudentPlans";
        public static readonly string FdStudentImgs = "StudentImages";
        public static readonly string FdTeacherImgs = "TeacherImages";
        public static readonly string FdHtmlCaches = "HtmlCaches";

        public static readonly string[] Folders = [
              FdStudentPrograms
            , FdStudentPlans
            , FdStudentImgs
            , FdTeacherImgs
            , FdHtmlCaches
        ];

        public abstract string CreateFolderIfNotExists(string path);
        public abstract void CreateFoldersAtStartUp();

        /// <summary>
        /// Xoá bỏ mọi file có trong folder.
        /// </summary>
        /// <param name="folderPath">Folder Path</param>
        public abstract void DelAllInThisFolder(string folderPath);
    }
}
