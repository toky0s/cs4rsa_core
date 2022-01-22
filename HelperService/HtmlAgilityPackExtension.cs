using HtmlAgilityPack;
using System.Net.Http;

namespace HelperService
{
    public class HtmlAgilityPackExtension
    {
        public static HtmlDocument GetHtmlDocumentWithHttpClient(string url)
        {
            using HttpClient client = new();
            using HttpResponseMessage response = client.GetAsync(url).Result;
            using HttpContent content = response.Content;
            string result = content.ReadAsStringAsync().Result;
            HtmlDocument document = new();
            document.LoadHtml(result);
            return document;
        }
    }
}
