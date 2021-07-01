using cs4rsa.Helpers;
using cs4rsa.BasicData;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using cs4rsa.Interfaces;

namespace cs4rsa.Crawler
{
    /// <summary>
    /// Lấy mã ngành của một sinh viên.
    /// </summary>
    public class CurriculumCrawler
    {
        public static Curriculum GetCurriculum(string specialString)
        {
            string t = Helpers.Helpers.GetTimeFromEpoch();
            string url = $"https://mydtu.duytan.edu.vn/Modules/curriculuminportal/ajax/LoadChuongTrinhHoc.aspx?t={t}&studentidnumber={specialString}";
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            HtmlNode docNode = doc.DocumentNode;
            HtmlNode scriptNode = docNode.SelectSingleNode("//td/script");
            HtmlNode nameNode = docNode.SelectSingleNode("//li/a");
            string name = StringHelper.CleanString(nameNode.InnerText);
            string content = scriptNode.InnerText;

            Regex regex = new Regex("curid=[0-9]*");
            Match matching = regex.Match(content);
            string matchString = matching.Value.ToString();
            string curid = matchString.Split(new char[] { '=' })[1];
            Curriculum curriculum = new Curriculum(curid, name);
            return curriculum;
        }
    }
}
