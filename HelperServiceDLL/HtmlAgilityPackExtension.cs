using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HelperService
{
    public class HtmlAgilityPackExtension
    {
        public static async Task<HtmlDocument> GetHtmlDocumentWithHttpClient(string url)
        {
            HttpClient client = new();
            string content = await client.GetStringAsync(url);
            HtmlDocument document = new();
            document.LoadHtml(content);
            return document;
        }
    }
}
