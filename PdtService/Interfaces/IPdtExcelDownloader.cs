
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdtService.Interfaces
{
    public interface IPdtExcelDownloader
    {
        Task<string> DownloadExcel(string url, string filePath);
    }
}
