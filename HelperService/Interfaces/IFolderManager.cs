using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperService.Interfaces
{
    public interface IFolderManager
    {
        string CreateFolderIfNotExists(string folderName);
    }
}
