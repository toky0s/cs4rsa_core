using cs4rsa_core.Utils.Interfaces;

using System.IO;

namespace cs4rsa_core.Utils
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
    }
}
