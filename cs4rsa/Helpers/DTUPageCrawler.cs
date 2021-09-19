using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.Helpers
{
    public class DtuPageCrawler
    {
        /// <summary>
        /// Lấy ra trang HTML của một trang MyDTU.
        /// </summary>
        /// <param name="sessionId">Giá trị của ASP.NET_SessionId trong cookies trang MyDTU.</param>
        /// <param name="url">Url của một trang thuộc MyDTU.</param>
        /// <returns></returns>
        public static string GetHtml(string sessionId, string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            CookieContainer cookieContainer = new CookieContainer();
            Cookie cookie = new Cookie("ASP.NET_SessionId", sessionId) { Domain = request.Host };
            cookieContainer.Add(cookie);
            request.CookieContainer = cookieContainer;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream;
                if (string.IsNullOrWhiteSpace(response.CharacterSet))
                    readStream = new StreamReader(receiveStream);
                else
                    readStream = new StreamReader(receiveStream,
                        Encoding.GetEncoding(response.CharacterSet));
                string data = readStream.ReadToEnd();
                response.Close();
                readStream.Close();
                return data;
            }
            return null;
        }
    }
}
