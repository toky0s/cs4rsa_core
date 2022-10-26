using cs4rsa_core.Utils.Interfaces;

using System;
using System.IO;

namespace cs4rsa_core.Utils
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
