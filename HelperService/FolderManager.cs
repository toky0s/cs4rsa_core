using HelperService.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
