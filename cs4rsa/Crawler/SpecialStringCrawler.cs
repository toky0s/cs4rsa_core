using HtmlAgilityPack;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace cs4rsa.Crawler
{
    /// <summary>
    /// Trình cào này thực hiện lấy chuỗi đặc biệt từ session id được truyền vào.
    /// </summary>
    public class SpecialStringCrawler
    {
        private static string _sessionId;
        public SpecialStringCrawler(string sessionId)
        {
            _sessionId = sessionId;
        }

        private static string GetHtml(string sessionId)
        {
            string urlAddress = "https://mydtu.duytan.edu.vn/sites/index.aspx?p=home_studyingwarning&functionid=113";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
            CookieContainer cookieContainer = new CookieContainer();
            Cookie cookie = new Cookie("ASP.NET_SessionId", sessionId) { Domain = request.Host };
            cookieContainer.Add(cookie);
            request.CookieContainer = cookieContainer;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;
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

        public static string GetSpecialString()
        {
            string html = GetHtml(_sessionId);
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            HtmlNode spanTag = htmlDocument.DocumentNode.SelectSingleNode("//*[@id=\"canhbaohoctap\"]/div[2]/table/tbody/tr/td[3]/span");
            if (spanTag != null)
            {
                string onClickValue = spanTag.Attributes["onclick"].Value;
                string[] split = onClickValue.Split(new string[] { "'" }, StringSplitOptions.RemoveEmptyEntries);
                string specialString = split[1];
                return specialString;
            }
            return null;
        }
    }
}
