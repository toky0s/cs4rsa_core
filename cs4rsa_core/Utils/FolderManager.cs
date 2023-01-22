using Cs4rsa.Utils.Interfaces;

using System.IO;

namespace Cs4rsa.Utils
{
    public class FolderManager : IFolderManager
    {
        public string CreateFolderIfNotExists(string path)
        {
            if (!Directory.Exists(path))
            {
                DirectoryInfo directory = Directory.CreateDirectory(path);
                return directory.FullName;
            }
            return path;
        }

        public static void CreateDirWhenFirstTimeStartApp()
        {

        }
    }
}
