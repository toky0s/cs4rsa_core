/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using CwebizAPI.Crawlers.CurriculumCrawlerSvc.Crawlers.Interfaces;

using HtmlAgilityPack;

using System.Text.RegularExpressions;
using CwebizAPI.Crawlers.StudentCrawlerSvc.Models;

namespace CwebizAPI.Crawlers.CurriculumCrawlerSvc.Crawlers
{
    /// <summary>
    /// Bộ cào mã ngành của một sinh viên.
    /// </summary>
    /// <remarks>
    /// Mã ngành bao gồm:
    /// - Mã số mã ngành: 241
    /// - Tên ngành: K24 - Công nghệ phần mềm
    /// 
    /// Tuỳ theo mã ngành mà số lượng môn học,
    /// các môn tiên quyết và song hành sẽ khác nhau.
    /// 
    /// Created Date: 21/06/2023
    /// Modified Date: 21/06/2023
    /// Author: Truong A Xin
    /// </remarks>
    public class CurriculumCrawler : BaseCrawler, ICurriculumCrawler
    {
        private readonly HtmlWeb _htmlWeb;

        public CurriculumCrawler(HtmlWeb htmlWeb)
        {
            _htmlWeb = htmlWeb;
        }

        public async Task<CrawledCurriculum> GetCurriculum(string? specialString)
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
            CrawledCurriculum curriculum = new() { CurriculumId = curid, Name = name };
            return curriculum;
        }
    }
}
