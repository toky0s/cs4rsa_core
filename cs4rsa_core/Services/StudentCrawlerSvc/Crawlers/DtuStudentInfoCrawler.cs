using Cs4rsa.BaseClasses;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;
using Cs4rsa.Services.CurriculumCrawlerSvc.Crawlers.Interfaces;
using Cs4rsa.Services.StudentCrawlerSvc.Crawlers.Interfaces;
using Cs4rsa.Utils;

using HtmlAgilityPack;

using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Cs4rsa.Services.StudentCrawlerSvc.Crawlers
{
    /// <summary>
    /// Deprecated
    /// </summary>
    public class DtuStudentInfoCrawler : BaseCrawler, IDtuStudentInfoCrawler
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurriculumCrawler _curriculumCrawler;
        private readonly HtmlWeb _htmlWeb;

        public DtuStudentInfoCrawler(
            IUnitOfWork unitOfWork,
            ICurriculumCrawler curriculumCrawler,
            HtmlWeb htmlWeb)
        {
            _unitOfWork = unitOfWork;
            _curriculumCrawler = curriculumCrawler;
            _htmlWeb = htmlWeb;
        }

        public async Task<Student> Crawl(string specialString)
        {
            string url = $"https://mydtu.duytan.edu.vn/Modules/mentor/WarningDetail.aspx?t={GetTimeFromEpoch()}&stid={specialString}";
            HtmlDocument doc = await _htmlWeb.LoadFromWebAsync(url);
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

            string sBirthday = StringHelper.ParseDateTime(birthdayNode.InnerText);
            if (!DateTime.TryParseExact(sBirthday, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime birthday))
            {
                if (!DateTime.TryParseExact(sBirthday, "dd/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out birthday))
                {
                    if (!DateTime.TryParseExact(sBirthday, "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out birthday))
                    {
                        DateTime.TryParseExact(sBirthday, "d/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out birthday);
                    }
                }
            }
            string cmnd = cmndNode.InnerText;
            string email = emailNode.InnerText;
            string phoneNumber = phoneNumberNode.InnerText;
            string address = StringHelper.SuperCleanString(addressNode.InnerText);

            string imageSrcData = imageNode.Attributes["src"].Value;
            string imageBase64Data = imageSrcData.Replace("data:image/jpg;base64,", string.Empty);

            Curriculum curriculum = await _curriculumCrawler.GetCurriculum(specialString);
            Curriculum existCurriculum = _unitOfWork.Curriculums.GetByID(curriculum.CurriculumId);
            if (existCurriculum == null)
            {
                _unitOfWork.Curriculums.Insert(curriculum);
            }
            Student studentExist = _unitOfWork.Students.GetByStudentId(studentId);
            if (studentExist == null)
            {
                Student student = new()
                {
                    Name = name,
                    StudentId = studentId,
                    SpecialString = specialString,
                    BirthDay = birthday,
                    Cmnd = cmnd,
                    Email = email,
                    PhoneNumber = phoneNumber,
                    Address = address,
                    //AvatarImgPath = imageBase64Data,
                    CurriculumId = curriculum.CurriculumId
                };
                _unitOfWork.Students.Add(student);
                return student;
            }
            return studentExist;
        }

        public Task<string> DownloadProfileImg(string studentCode)
        {
            throw new NotImplementedException();
        }
    }
}
