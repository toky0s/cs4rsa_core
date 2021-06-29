using cs4rsa.Helpers;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace cs4rsa.Crawler
{
    public class CurIdCrawler
    {
        public static string GetCurId(string specialString)
        {
            string t = Helpers.Helpers.GetTimeFromEpoch();
            string url = $"https://mydtu.duytan.edu.vn/Modules/curriculuminportal/ajax/LoadChuongTrinhHoc.aspx?t={t}&studentidnumber={specialString}";
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            HtmlNode docNode = doc.DocumentNode;
            HtmlNode scriptNode = docNode.SelectSingleNode("//td/script");
            string content = scriptNode.InnerText;

            Regex regex = new Regex("curid=[0-9]*");
            Match matching = regex.Match(content);
            string matchString = matching.Value.ToString();
            string curid = matchString.Split(new char[] { '=' })[1];
            return curid;
        }
    }
}
