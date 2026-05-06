using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Velopack;

namespace Cs4rsa.App.Services
{
    public interface IUpdateService
    {
        Task<UpdateInfo> HasNewVersion();
        Task UpdateNewVersion(UpdateInfo newVersion, Action<int> updateProgress, CancellationToken token);
    }
}
