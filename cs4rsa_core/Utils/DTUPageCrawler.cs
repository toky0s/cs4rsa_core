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
        /// 
        /// Update date:
        /// 23/12/2022 - Trong trường hợp có lỗi xảy ra, một chuỗi rỗng sẽ được trả về.
        /// 25/12/2022 - MyDTU có một cơ chế chặn request liên tục sau 14 request.
        /// 
        /// Author:
        /// toky0s
        /// </summary>
        /// <param name="sessionId">Giá trị của ASP.NET_SessionId trong cookies trang MyDTU.</param>
        /// <param name="url">Url của một trang thuộc MyDTU.</param>
        /// <returns>Trang HTML.</returns>
        public static async Task<string> GetHtml(string sessionId, string url)
        {
            try
            {
                Uri baseAddress = new(url);

                CookieContainer cookieContainer = new();
                cookieContainer.Add(baseAddress, new Cookie("ASP.NET_SessionId", sessionId));
                using HttpClientHandler handler = new() { CookieContainer = cookieContainer };

                using HttpClient client = new(handler)
                {
                    BaseAddress = baseAddress
                };

                HttpResponseMessage message = await client.GetAsync(url);
                message.EnsureSuccessStatusCode();
                Stream receiveStream = await message.Content.ReadAsStreamAsync();
                StreamReader readStream = new(receiveStream);
                string data = await readStream.ReadToEndAsync();
                readStream.Close();
                return data;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
