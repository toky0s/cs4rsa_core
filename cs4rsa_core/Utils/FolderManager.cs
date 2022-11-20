using Cs4rsa.Utils.Interfaces;

using System;
using System.IO;

namespace Cs4rsa.Utils
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
