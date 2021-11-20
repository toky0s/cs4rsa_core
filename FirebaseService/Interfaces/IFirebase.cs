using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirebaseService.Interfaces
{
    public interface IFirebase
    {
        Task<string> GetLatestVersion();
        Task<bool> PostUser(string studentId, string specialString);
    }
}
