using CwebizAPI.Crawlers.TeacherCrawlerSvc.Crawlers.Interfaces;
using CwebizAPI.Crawlers.TeacherCrawlerSvc.Models;
using CwebizAPI.Db.Interfaces;
using CwebizAPI.Share.Database.Models;
using CwebizAPI.Utils;
using HtmlAgilityPack;

namespace CwebizAPI.Crawlers.TeacherCrawlerSvc.Crawlers
{
    public class TeacherCrawler : ITeacherCrawler
    {
        #region Services

        private readonly IUnitOfWork _unitOfWork;
        private readonly HtmlWeb _htmlWeb;

        #endregion

        public TeacherCrawler(
            IUnitOfWork unitOfWork,
            HtmlWeb htmlWeb)
        {
            _unitOfWork = unitOfWork;
            _htmlWeb = htmlWeb;
        }

        private static string GetIntructorId(string url)
        {
            string[] slideChars = { "&" };
            string[] separatingStrings = { "=" };
            string intructorIdParam = url.Split(slideChars, StringSplitOptions.RemoveEmptyEntries)[2];
            return intructorIdParam.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries)[1];
        }

        public async Task<TeacherModel> Crawl(string url, int courseId)
        {
            if (url == null)
            {
                return null;
            }

            int teacherId = int.Parse(GetIntructorId(url));
            TeacherModel teacherModel;

            HtmlDocument _htmlDocument = await _htmlWeb.LoadFromWebAsync(url);
            if (_htmlDocument == null)
            {
                return null;
            }

            HtmlNodeCollection infoNodes = _htmlDocument
                .DocumentNode
                .SelectNodes("//span[contains(@class, 'info_gv')]");
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

            /// Tham khảo CS0026 https://github.com/toky0s/cs4rsa_core/issues/37
            IEnumerable<string> teachedSubjects = _htmlDocument.DocumentNode
                .SelectNodes(xpathLiNode)
                .Select(item => StringHelper.SuperCleanString(item.InnerText));


            Teacher teacher = new()
            {
                TeacherId = id,
                Name = name,
                Sex = sex,
                Place = place,
                Degree = degree,
                WorkUnit = workUnit,
                Position = position,
                Subject = subject,
                Form = form,
            };
            _unitOfWork.TeacherRepository.Add(teacher);
            teacherModel = new TeacherModel(teacher);
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
    }
}