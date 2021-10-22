using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using TeacherCrawlerService1.Crawlers.Interfaces;
using Cs4rsaDatabaseService.DataProviders;
using Cs4rsaDatabaseService.Models;
using HelperService;
using Cs4rsaDatabaseService.Interfaces;
using System.Threading.Tasks;

namespace TeacherCrawlerService1.Crawlers
{
    public class TeacherCrawler : ITeacherCrawler
    {
        private HtmlDocument _htmlDocument;
        private IUnitOfWork _unitOfWork;
        public TeacherCrawler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
            return _unitOfWork.Teachers.GetById(instructorId) is Teacher;
        }

        public async Task<Teacher> Crawl(string url)
        {
            if (url != null)
            {
                int teacherId = int.Parse(GetIntructorId(url));
                if (IsTeacherHasInDatabase(teacherId))
                {
                    return _unitOfWork.Teachers.GetById(teacherId);
                }
                else
                {
                    HtmlWeb web = new HtmlWeb();
                    _htmlDocument = await web.LoadFromWebAsync(url);
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
                        Teacher teacher = new()
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
                        _unitOfWork.Teachers.Add(teacher);
                        _unitOfWork.Complete();
                        return teacher;
                    }
                    return null;
                }
            }
            return null;
        }
    }
}
