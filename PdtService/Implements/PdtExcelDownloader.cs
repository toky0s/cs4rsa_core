using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using PdtService.Interfaces;

namespace PdtService.Implements
{
    public class PdtExcelDownloader : IPdtExcelDownloader
    {
        public async Task<string> DownloadExcel(string url, string filePath)
        {
            try
            {
                HttpClient client = new();
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using FileStream fs = new(filePath, FileMode.CreateNew);
                    await response.Content.CopyToAsync(fs);
                    return filePath;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return string.Empty;
            }
            return string.Empty;
        }
    }
}
