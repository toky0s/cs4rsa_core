using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using TeacherCrawlerService1.Crawlers.Interfaces;
using Cs4rsaDatabaseService.DataProviders;
using Cs4rsaDatabaseService.Models;
using HelperService;

namespace TeacherCrawlerService1.Crawlers
{
    public class TeacherCrawler : ITeacherCrawler
    {
        private string _url;
        private HtmlDocument _htmlDocument;
        private Cs4rsaDbContext _cs4rsaDbContext;
        public TeacherCrawler(string url, Cs4rsaDbContext cs4rsaDbContext)
        {
            _url = url;
            _cs4rsaDbContext = cs4rsaDbContext;
        }

        private static string GetIntructorId(string url)
        {
            string[] slideChars = { "&" };
            string[] separatingStrings = { "=" };
            string intructorIdParam = url.Split(slideChars, StringSplitOptions.RemoveEmptyEntries)[2];
            return intructorIdParam.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries)[1];
        }

        /// <summary>
        /// Kiểm tra xem một teacher với mã có tồn tại trong database hay không.
        /// </summary>
        /// <param name="instructorId"></param>
        /// <returns></returns>
        private bool IsTeacherHasInDatabase(int instructorId)
        {
            return _cs4rsaDbContext.Teachers.Where(teacher => teacher.TeacherId == instructorId).Any();
        }

        public Teacher Crawl()
        {
            if (_url != null)
            {
                int intructorId = int.Parse(GetIntructorId(_url));
                if (IsTeacherHasInDatabase(intructorId))
                {
                    return _cs4rsaDbContext.Teachers.Where(teacher => teacher.TeacherId == intructorId).FirstOrDefault();
                }
                else
                {
                    HtmlWeb web = new HtmlWeb();
                    _htmlDocument = web.Load(_url);
                    if (_htmlDocument != null)
                    {
                        List<HtmlNode> infoNodes = _htmlDocument.DocumentNode.SelectNodes("//span[contains(@class, 'info_gv')]").ToList();
                        string id = StringHelper.SuperCleanString(infoNodes[0].InnerText);
                        string name = StringHelper.SuperCleanString(infoNodes[1].InnerText);
                        string sex = StringHelper.SuperCleanString(infoNodes[2].InnerText);
                        string place = StringHelper.SuperCleanString(infoNodes[3].InnerText);
                        string degree = StringHelper.SuperCleanString(infoNodes[4].InnerText);
                        string workUnit = StringHelper.SuperCleanString(infoNodes[5].InnerText);
                        string position = StringHelper.SuperCleanString(infoNodes[6].InnerText);
                        string subject = StringHelper.SuperCleanString(infoNodes[7].InnerText);
                        string form = StringHelper.SuperCleanString(infoNodes[8].InnerText);
                        string xpathLiNode = "//ul[contains(@class, 'thugio')]/li";
                        List<HtmlNode> liNodes = _htmlDocument.DocumentNode.SelectNodes(xpathLiNode).ToList();
                        List<string> teachedSubjects = liNodes.Select(item => item.InnerText).ToList();
                        Teacher teacher = new Teacher
                        {
                            TeacherId = int.Parse(id),
                            Name = name,
                            Sex = sex,
                            Place = place,
                            Degree = degree,
                            WorkUnit = workUnit,
                            Position = position,
                            Subject = subject,
                            Form = form,
                        };
                        _cs4rsaDbContext.Teachers.Add(teacher);
                        _cs4rsaDbContext.SaveChanges();
                        return teacher;
                    }
                    return null;
                }
            }
            return null;
        }
    }
}
