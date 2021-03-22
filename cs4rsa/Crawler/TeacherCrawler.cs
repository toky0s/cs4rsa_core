using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using cs4rsa.BasicData;
using cs4rsa.Helpers;

namespace cs4rsa.Crawler
{
    public class TeacherCrawler
    {
        //http://courses.duytan.edu.vn/Sites/Home_ChuongTrinhDaoTao.aspx?p=home_lecturerdetail&timespan=71&intructorid=010132007&classid=139631&academicleveltypeid=&curriculumid=
        private HtmlDocument document;
        public TeacherCrawler(string url)
        {
            HtmlWeb web = new HtmlWeb();
            document = web.Load(url);
        }

        public Teacher ToTeacher()
        {
            List<HtmlNode> infoNodes = document.DocumentNode.SelectNodes("//span[contains(@class, 'info_gv')]").ToList();
            string id = Helpers.StringHelper.SuperCleanString(infoNodes[0].InnerText);
            string name = Helpers.StringHelper.SuperCleanString(infoNodes[1].InnerText);
            string sex = Helpers.StringHelper.SuperCleanString(infoNodes[2].InnerText);
            string place = Helpers.StringHelper.SuperCleanString(infoNodes[3].InnerText);
            string degree = Helpers.StringHelper.SuperCleanString(infoNodes[4].InnerText);
            string workUnit = Helpers.StringHelper.SuperCleanString(infoNodes[5].InnerText);
            string position = Helpers.StringHelper.SuperCleanString(infoNodes[6].InnerText);
            string subject = Helpers.StringHelper.SuperCleanString(infoNodes[7].InnerText);
            string form = Helpers.StringHelper.SuperCleanString(infoNodes[8].InnerText);
            string xpathLiNode = "//ul[contains(@class, 'thugio')]/li";
            List<HtmlNode> liNodes = document.DocumentNode.SelectNodes(xpathLiNode).ToList();
            string[] teachedSubjects = liNodes.Select(item => item.InnerText).ToArray();
            return new Teacher(id, name, sex, place, degree, workUnit, position, subject, form, teachedSubjects);
        } 
    }
}
