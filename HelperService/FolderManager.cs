using HelperService.Interfaces;
using System;
using System.IO;

namespace HelperService
{
    public class FolderManager : IFolderManager
    {
        public string CreateFolderIfNotExists(string folderName)
        {
            string pathToNewFolder = Path.Combine(AppContext.BaseDirectory, folderName);
            DirectoryInfo directory = Directory.CreateDirectory(pathToNewFolder);
            return directory.FullName;
        }
    }
}
