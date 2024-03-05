using Cs4rsa.Common.Interfaces;

using System;
using System.IO;

namespace Cs4rsa.Common
{
    public class FolderManager : IFolderManager
    {
        public override void CreateFoldersAtStartUp()
        {
            for (var i = 0; i < Folders.Length; ++i)
            {
                var path = Path.Combine(AppContext.BaseDirectory, Folders[i]);
                CreateFolderIfNotExists(path);
            }
        }

        public override string CreateFolderIfNotExists(string path)
        {
            if (!Directory.Exists(path))
            {
                var directory = Directory.CreateDirectory(path);
                return directory.FullName;
            }
            return path;
        }

        public override void DelAllInThisFolder(string folderPath)
        {
            var filePaths = Directory.GetFiles(folderPath);
            for (var i = 0; i < filePaths.Length; i++)
            {
                File.Delete(filePaths[i]);
            }
        }
    }
}
