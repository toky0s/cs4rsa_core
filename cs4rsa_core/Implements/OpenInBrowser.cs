using cs4rsa_core.Interfaces;
using System.Diagnostics;

namespace cs4rsa_core.Implements
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
