using Cs4rsa.Utils.Interfaces;

using System;
using System.IO;

namespace Cs4rsa.Utils
{
    public class FolderManager : IFolderManager
    {
        public void CreateFoldersAtStartUp()
        {
            for (int i = 0; i < IFolderManager.FOLDERS.Length; ++i)
            {
                string path = Path.Combine(AppContext.BaseDirectory, IFolderManager.FOLDERS[i]);
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
    }
}
