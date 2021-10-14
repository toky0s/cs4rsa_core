using System;
using HtmlAgilityPack;
using HelperService;
using StudentCrawlerService.Interfaces;
using CurriculumCrawlerService.Crawlers;
using Cs4rsaDatabaseService.Models;
using Cs4rsaDatabaseService.DataProviders;
using System.Linq;
using Cs4rsaDatabaseService;

namespace StudentCrawlerService.Crawlers
{
    public class DtuStudentInfoCrawler: IDtuStudentInfoCrawler
    {
        private Cs4rsaDbContext _cs4rsaDbContext;

        public DtuStudentInfoCrawler(Cs4rsaDbContext cs4rsaDbContext)
        {
            _cs4rsaDbContext = cs4rsaDbContext;
        }

        public Student Crawl(string specialString)
        {
            string url = $"https://mydtu.duytan.edu.vn/Modules/mentor/WarningDetail.aspx?t={Helpers.GetTimeFromEpoch()}&stid={specialString}";
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            HtmlNode docNode = doc.DocumentNode;

            HtmlNode nameNode = docNode.SelectSingleNode("//div/table[1]/tr[1]/td[2]/strong");
            HtmlNode studentIdNode = docNode.SelectSingleNode("//div/table[1]/tr[2]/td[2]");
            HtmlNode birthdayNode = docNode.SelectSingleNode("//tr[3]/td[2]");
            HtmlNode cmndNode = docNode.SelectSingleNode("//tr[4]/td[2]");
            HtmlNode emailNode = docNode.SelectSingleNode("//tr[5]/td[2]");
            HtmlNode phoneNumberNode = docNode.SelectSingleNode("//tr[6]/td[2]");
            HtmlNode addressNode = docNode.SelectSingleNode("//tr[7]/td[2]");
            HtmlNode imageNode = docNode.SelectSingleNode("//*[@id=\"Image1\"]");

            string name = nameNode.InnerText;
            string studentId = studentIdNode.InnerText;
            
            string sBirthday = StringHelper.SuperCleanString(birthdayNode.InnerText);
            DateTime birthday = DateTime.ParseExact(sBirthday, "dd/MM/yyyy", null);

            string cmnd = cmndNode.InnerText;
            string email = emailNode.InnerText;
            string phoneNumber = phoneNumberNode.InnerText;
            string address = StringHelper.SuperCleanString(addressNode.InnerText);

            string imageSrcData = imageNode.Attributes["src"].Value;
            string imageBase64Data = imageSrcData.Replace("data:image/jpg;base64,", "");

            Curriculum curriculum = CurriculumCrawler.GetCurriculum(specialString);
            bool existCurriculum =_cs4rsaDbContext.Curriculums.Any(cr => cr.CurriculumId==curriculum.CurriculumId);
            if (!existCurriculum)
            {
                _cs4rsaDbContext.Curriculums.Add(curriculum);
                _cs4rsaDbContext.SaveChanges();
            }

            Student info = new()
            {
                Name = name,
                StudentId = studentId,
                SpecialString = specialString,
                BirthDay = birthday,
                Cmnd = cmnd,
                Email = email,
                PhoneNumber = phoneNumber,
                Address = address,
                AvatarImage = imageBase64Data,
                CurriculumId = curriculum.CurriculumId
            };

            bool existStudent = _cs4rsaDbContext.Students.Any(student => student.StudentId == info.StudentId);
            if (!existStudent)
            {
                _cs4rsaDbContext.Students.Add(info);
                _cs4rsaDbContext.SaveChanges();
            }
            return info;
        }
    }
}
