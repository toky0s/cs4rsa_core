using Cs4rsa.Utils.Interfaces;

using System.Diagnostics;

namespace Cs4rsa.Utils
{
    public class OpenInBrowser : IOpenInBrowser
    {
        public void Open(string url)
        {
            _ = Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
    }
}
