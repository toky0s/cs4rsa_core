using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Cs4rsa.Utils
{
    public class DtuPageCrawler
    {
        /// <summary>
        /// Lấy ra trang HTML của một trang MyDTU.
        /// </summary>
        /// <param name="sessionId">Giá trị của ASP.NET_SessionId trong cookies trang MyDTU.</param>
        /// <param name="url">Url của một trang thuộc MyDTU.</param>
        /// <returns>Trang HTML.</returns>
        public static async Task<string> GetHtml(string sessionId, string url)
        {
            Uri baseAddress = new(url);
            CookieContainer cookieContainer = new();
            using HttpClientHandler handler = new() { CookieContainer = cookieContainer };
            using HttpClient client = new(handler) { BaseAddress = baseAddress };
            cookieContainer.Add(baseAddress, new Cookie("ASP.NET_SessionId", sessionId));
            HttpResponseMessage message = await client.GetAsync(url);
            message.EnsureSuccessStatusCode();
            Stream receiveStream = await message.Content.ReadAsStreamAsync();
            StreamReader readStream;
            readStream = new StreamReader(receiveStream);
            string data = readStream.ReadToEnd();
            readStream.Close();
            return data;
        }
    }
}
