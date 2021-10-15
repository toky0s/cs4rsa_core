using Cs4rsaDatabaseService.Models;
using HelperService;
using HtmlAgilityPack;
using System;
using System.Text.RegularExpressions;

namespace CurriculumCrawlerService.Crawlers
{
    /// <summary>
    /// Lấy mã ngành của một sinh viên.
    /// </summary>
    public class CurriculumCrawler
    {
        public static Curriculum GetCurriculum(string specialString)
        {
            string t = Helpers.GetTimeFromEpoch();
            string url = $"https://mydtu.duytan.edu.vn/Modules/curriculuminportal/ajax/LoadChuongTrinhHoc.aspx?t={t}&studentidnumber={specialString}";
            HtmlWeb web = new();
            HtmlDocument doc = web.Load(url);
            HtmlNode docNode = doc.DocumentNode;
            HtmlNode scriptNode = docNode.SelectSingleNode("//td/script");
            HtmlNode nameNode = docNode.SelectSingleNode("//li/a");
            string name = StringHelper.CleanString(nameNode.InnerText);
            string content = scriptNode.InnerText;

            Regex regex = new("curid=[0-9]*");
            Match matching = regex.Match(content);
            string matchString = matching.Value.ToString();
            string sCurid = matchString.Split(new char[] { '=' })[1];
            int curid = int.Parse(sCurid);
            Curriculum curriculum = new() { CurriculumId=curid, Name=name};
            return curriculum;
        }
    }
}
