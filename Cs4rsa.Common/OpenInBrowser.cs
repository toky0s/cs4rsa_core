using Cs4rsa.Common.Interfaces;

using System.Diagnostics;
using System.IO;

namespace Cs4rsa.Common
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

        public void OpenFolderAndSelect(string path)
        {
            if (!File.Exists(path))
            {
                return;
            }
            var arg = "/select, \"" + path + "\"";
            Process.Start("explorer.exe", arg);
        }
    }
}
