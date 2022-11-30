using Cs4rsa.BaseClasses;
using Cs4rsa.Cs4rsaDatabase.Models;
using Cs4rsa.Services.CurriculumCrawlerSvc.Crawlers.Interfaces;

using HtmlAgilityPack;

using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cs4rsa.Services.CurriculumCrawlerSvc.Crawlers
{
    /// <summary>
    /// Lấy mã ngành của một sinh viên.
    /// </summary>
    public class CurriculumCrawler : BaseCrawler, ICurriculumCrawler
    {
        private readonly HtmlWeb _htmlWeb;

        public CurriculumCrawler(HtmlWeb htmlWeb)
        {
            _htmlWeb = htmlWeb;
        }

        public async Task<Curriculum> GetCurriculum(string specialString)
        {
            string url = $"https://mydtu.duytan.edu.vn/Modules/curriculuminportal/ajax/LoadChuongTrinhHoc.aspx?t={GetTimeFromEpoch()}&studentidnumber={specialString}";
            HtmlDocument doc = await _htmlWeb.LoadFromWebAsync(url);
            HtmlNode docNode = doc.DocumentNode;
            HtmlNode scriptNode = docNode.SelectSingleNode("//td/script");
            HtmlNode nameNode = docNode.SelectSingleNode("//li/a");
            string name = nameNode.InnerText.Trim();
            string content = scriptNode.InnerText;

            Regex regex = new("curid=[0-9]*");
            Match matching = regex.Match(content);
            string matchString = matching.Value.ToString();
            string sCurid = matchString.Split(new char[] { '=' })[1];
            int curid = int.Parse(sCurid);
            Curriculum curriculum = new() { CurriculumId = curid, Name = name };
            return curriculum;
        }
    }
}
