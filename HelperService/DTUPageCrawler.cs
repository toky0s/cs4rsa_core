using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HelperService
{
    public class DtuPageCrawler
    {
        /// <summary>
        /// Lấy ra trang HTML của một trang MyDTU.
        /// </summary>
        /// <param name="sessionId">Giá trị của ASP.NET_SessionId trong cookies trang MyDTU.</param>
        /// <param name="url">Url của một trang thuộc MyDTU.</param>
        /// <returns></returns>
        public static async Task<string> GetHtml(string sessionId, string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            CookieContainer cookieContainer = new();
            Cookie cookie = new Cookie("ASP.NET_SessionId", sessionId) { Domain = request.Host };
            cookieContainer.Add(cookie);
            request.CookieContainer = cookieContainer;

            WebResponse response = await request.GetResponseAsync();
            HttpWebResponse httpWebResponse = (HttpWebResponse)response;

            if (httpWebResponse.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream;
                if (string.IsNullOrWhiteSpace(httpWebResponse.CharacterSet))
                    readStream = new StreamReader(receiveStream);
                else
                    readStream = new StreamReader(receiveStream,
                        Encoding.GetEncoding(httpWebResponse.CharacterSet));
                string data = readStream.ReadToEnd();
                response.Close();
                readStream.Close();
                return data;
            }
            return null;
        }
    }
}
