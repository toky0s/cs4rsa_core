using cs4rsa_core.Utils.Interfaces;

using System.Diagnostics;

namespace cs4rsa_core.Utils
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
