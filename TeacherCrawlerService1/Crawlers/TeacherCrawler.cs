using Cs4rsaDatabaseService.Interfaces;
using Cs4rsaDatabaseService.Models;

using HelperService;
using HelperService.Interfaces;

using HtmlAgilityPack;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using TeacherCrawlerService1.Crawlers.Interfaces;

namespace TeacherCrawlerService1.Crawlers
{
    public class TeacherCrawler : ITeacherCrawler
    {
        #region Properties
        private readonly string _strSavingTeacherImageFolderPath;
        #endregion

        #region Services
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFolderManager _folderManager;
        #endregion

        public TeacherCrawler(IUnitOfWork unitOfWork, IFolderManager folderManager)
        {
            _unitOfWork = unitOfWork;
            _folderManager = folderManager;

            _strSavingTeacherImageFolderPath = _folderManager.CreateFolderIfNotExists("TeacherImages");
        }

        private static string GetIntructorId(string url)
        {
            string[] slideChars = { "&" };
            string[] separatingStrings = { "=" };
            string intructorIdParam = url.Split(slideChars, StringSplitOptions.RemoveEmptyEntries)[2];
            return intructorIdParam.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries)[1];
        }

        private bool IsTeacherHasInDatabase(int instructorId)
        {
            return _unitOfWork.Teachers.GetById(instructorId) is not null;
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
                    HtmlWeb web = new();
                    HtmlDocument _htmlDocument = await web.LoadFromWebAsync(url);
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
                        string strPath = await OnDownloadImage(id);
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
                            Path = strPath
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

        private static string GetTeacherImagePath(string teacherId)
        {
            return $"http://hfs1.duytan.edu.vn/Upload/dichvu/gv_{teacherId}_01.jpg";
        }

        private static string GetTeacherImageName(string teacherId)
        {
            return $"gv_{teacherId}_01.jpg";
        }

        private async Task<string> OnDownloadImage(string teacherId)
        {
            string imageUrl = GetTeacherImagePath(teacherId);
            string imageName = GetTeacherImageName(teacherId);
            string strImageFullPath = Path.Combine(_strSavingTeacherImageFolderPath, imageName);
            using HttpClient httpClient = new();
            HttpResponseMessage response = await httpClient.GetAsync(imageUrl);
            if (response.IsSuccessStatusCode)
            {
                using WebClient webClient = new();
                webClient.DownloadFileAsync(new Uri(imageUrl), strImageFullPath);
                return strImageFullPath;
            }
            return "";
        }
    }
}
