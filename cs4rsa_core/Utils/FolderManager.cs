using Cs4rsa.Utils.Interfaces;

using System;
using System.IO;

namespace Cs4rsa.Utils
{
    public class FolderManager : IFolderManager
    {
        public void CreateFoldersAtStartUp()
        {
            for (int i = 0; i < IFolderManager.Folders.Length; ++i)
            {
                string path = Path.Combine(AppContext.BaseDirectory, IFolderManager.Folders[i]);
                CreateFolderIfNotExists(path);
            }
        }

        public string CreateFolderIfNotExists(string path)
        {
            if (!Directory.Exists(path))
            {
                DirectoryInfo directory = Directory.CreateDirectory(path);
                return directory.FullName;
            }
            return path;
        }

        public void DelAllInThisFolder(string folderPath)
        {
            string[] filePaths = Directory.GetFiles(folderPath);
            for (int i = 0; i < filePaths.Length; i++)
            {
                File.Delete(filePaths[i]);
            }
        }
    }
}
