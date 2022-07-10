using HelperService.Interfaces;

using System;
using System.IO;

namespace HelperService
{
    public class FolderManager : IFolderManager
    {
        /// <summary>
        /// Tạo mới một thư mục nằm cùng thư mục với ứng dụng gốc.
        /// </summary>
        /// <param name="folderName">Tên thư mục</param>
        /// <returns>Đường dẫn tuyệt đối tới thư mục</returns>
        public string CreateFolderIfNotExists(string folderName)
        {
            string pathToNewFolder = Path.Combine(AppContext.BaseDirectory, folderName);
            DirectoryInfo directory = Directory.CreateDirectory(pathToNewFolder);
            return directory.FullName;
        }
    }
}
