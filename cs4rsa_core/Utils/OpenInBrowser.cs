using Cs4rsa.Utils.Interfaces;

using System.Diagnostics;
using System.IO;

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

        public void OpenFolderAndSelect(string path)
        {
            if (!File.Exists(path))
            {
                return;
            }
            string arg = "/select, \"" + path + "\"";
            Process.Start("explorer.exe", arg);
        }
    }
}
