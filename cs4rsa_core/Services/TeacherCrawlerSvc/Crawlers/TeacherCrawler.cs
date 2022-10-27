using cs4rsa_core.Cs4rsaDatabase.Interfaces;
using cs4rsa_core.Cs4rsaDatabase.Models;
using cs4rsa_core.Services.TeacherCrawlerSvc.Crawlers.Interfaces;
using cs4rsa_core.Utils;
using cs4rsa_core.Utils.Interfaces;

using HtmlAgilityPack;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace cs4rsa_core.Services.TeacherCrawlerSvc.Crawlers
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

        private async Task<bool> IsTeacherHasInDatabase(int instructorId)
        {
            return await _unitOfWork.Teachers.GetByIdAsync(instructorId) is not null;
        }

        public async Task<Teacher> Crawl(string url)
        {
            if (url != null)
            {
                int teacherId = int.Parse(GetIntructorId(url));
                if (await IsTeacherHasInDatabase(teacherId))
                {
                    return await _unitOfWork.Teachers.GetByIdAsync(teacherId);
                }
                else
                {
                    HtmlWeb web = new();
                    HtmlDocument _htmlDocument = await web.LoadFromWebAsync(url);
                    if (_htmlDocument != null)
                    {
                        HtmlNodeCollection infoNodes = _htmlDocument.DocumentNode.SelectNodes("//span[contains(@class, 'info_gv')]");
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

                        /// TODO: Danh sách môn học giảng viên đang dạy
                        /// Tham khảo CS0026 https://github.com/toky0s/cs4rsa_core/issues/37
                        IEnumerable<string> teachedSubjects = _htmlDocument.DocumentNode
                            .SelectNodes(xpathLiNode)
                            .Select(item => item.InnerText);

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
                        await _unitOfWork.Teachers.AddAsync(teacher);
                        await _unitOfWork.CompleteAsync();
                        return teacher;
                    }
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
            return string.Empty;
        }
    }
}
