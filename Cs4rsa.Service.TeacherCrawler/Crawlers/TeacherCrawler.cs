using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cs4rsa.Common;
using Cs4rsa.Service.TeacherCrawler.Crawlers.Interfaces;
using Cs4rsa.Service.TeacherCrawler.Models;
using HtmlAgilityPack;

namespace Cs4rsa.Service.TeacherCrawler.Crawlers
{
    public class TeacherCrawler : ITeacherCrawler
    {
        private readonly HtmlWeb _htmlWeb;

        public TeacherCrawler(HtmlWeb htmlWeb)
        {
            _htmlWeb = htmlWeb;
        }

        public async Task<TeacherModel> Crawl(string url, int courseId, bool isUpdate)
        {
            if (url == null)
            {
                return null;
            }
            
            var htmlDocument = await _htmlWeb.LoadFromWebAsync(url);
            if (htmlDocument == null)
            {
                return null;
            }
            var infoNodes = htmlDocument
                .DocumentNode
                .SelectNodes("//span[contains(@class, 'info_gv')]");
            var id = infoNodes[0].InnerText.SuperCleanString();
            var name = infoNodes[1].InnerText.SuperCleanString();
            var sex = infoNodes[2].InnerText.SuperCleanString();
            var place = infoNodes[3].InnerText.SuperCleanString();
            var degree = infoNodes[4].InnerText.SuperCleanString();
            var workUnit = infoNodes[5].InnerText.SuperCleanString();
            var position = infoNodes[6].InnerText.SuperCleanString();
            var subject = infoNodes[7].InnerText.SuperCleanString();
            var form = infoNodes[8].InnerText.SuperCleanString();
            const string xPathLiNode = "//ul[contains(@class, 'thugio')]/li";

            // Tham khảo CS0026 https://github.com/toky0s/cs4rsa_core/issues/37
            var taughtSubjects = htmlDocument.DocumentNode
                .SelectNodes(xPathLiNode)
                .Select(item => item.InnerText.SuperCleanString())
                .ToArray();
            
            var teacherModel = new TeacherModel
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
                TaughtSubjects = taughtSubjects,
                Url = url
            };

            return teacherModel;
        }

        private static string GetTeacherImagePath(string teacherId)
        {
            return $"http://hfs1.duytan.edu.vn/Upload/dichvu/gv_{teacherId}_01.jpg";
        }

        private static string GetTeacherImageName(string teacherId)
        {
            return $"gv_{teacherId}_01.jpg";
        }

        public async Task<string> OnDownloadImage(string teacherId, string folder)
        {
            var imageUrl = GetTeacherImagePath(teacherId);
            var imageName = GetTeacherImageName(teacherId);
            var strImageFullPath = Path.Combine(folder, imageName);
            File.Delete(strImageFullPath);
            var result = await ImageDownloader.DownloadImage(imageUrl, strImageFullPath);
            return result ? strImageFullPath : string.Empty;
        }
    }
}
