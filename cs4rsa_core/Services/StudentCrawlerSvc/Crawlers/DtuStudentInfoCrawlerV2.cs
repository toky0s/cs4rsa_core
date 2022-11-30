using Cs4rsa.BaseClasses;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;
using Cs4rsa.Services.CurriculumCrawlerSvc.Crawlers.Interfaces;
using Cs4rsa.Services.StudentCrawlerSvc.Crawlers.Interfaces;
using Cs4rsa.Utils;

using HtmlAgilityPack;

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Cs4rsa.Services.StudentCrawlerSvc.Crawlers
{
    /// <summary>
    /// 23/1/2022 Version 2 DTU Student Info Crawler
    /// - Fix XPath
    /// - Fix Get Image
    /// </summary>
    public class DtuStudentInfoCrawlerV2 : BaseCrawler, IDtuStudentInfoCrawler
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurriculumCrawler _curriculumCrawler;
        private readonly HtmlWeb _htmlWeb;

        public DtuStudentInfoCrawlerV2(
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

            HtmlNode nameNode = docNode.SelectSingleNode("//tr/td[2]/table/tr[1]/td[2]/strong");
            HtmlNode studentIdNode = docNode.SelectSingleNode("//tr/td[2]/table/tr[2]/td[2]");
            HtmlNode birthdayNode = docNode.SelectSingleNode("//tr[3]/td[2]");
            HtmlNode cmndNode = docNode.SelectSingleNode("//tr[4]/td[2]");
            HtmlNode emailNode = docNode.SelectSingleNode("//tr[5]/td[2]");
            HtmlNode phoneNumberNode = docNode.SelectSingleNode("//tr[6]/td[2]");
            HtmlNode addressNode = docNode.SelectSingleNode("//tr[7]/td[2]");
            string imageId = $"imgDetail_{studentIdNode.InnerText}";
            HtmlNode imageNode = docNode.SelectSingleNode($"//*[@id=\"{imageId}\"]");

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
            Uri imageSrc = new(imageSrcData);
            string imageBase64Data = await LoadImage(imageSrc);

            Curriculum curriculum = await _curriculumCrawler.GetCurriculum(specialString);
            Curriculum existCurriculum = await _unitOfWork.Curriculums.GetByIdAsync(curriculum.CurriculumId);

            if (existCurriculum == null)
            {
                await _unitOfWork.Curriculums.AddAsync(curriculum);
                await _unitOfWork.CompleteAsync();
            }
            Student studentExist = await _unitOfWork.Students.GetByStudentIdAsync(studentId);
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
                    AvatarImage = imageBase64Data,
                    CurriculumId = curriculum.CurriculumId
                };
                await _unitOfWork.BeginTransAsync();
                await _unitOfWork.Students.AddAsync(student);
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitAsync();
                return student;
            }
            return studentExist;
        }

        public async static Task<string> LoadImage(Uri uri)
        {
            try
            {
                using HttpClient client = new();
                using var response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                Stream stream = await response.Content.ReadAsStreamAsync();
                return ConvertToBase64(stream);

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to load the image: {0}", ex.Message);
            }

            return null;
        }

        public static string ConvertToBase64(Stream stream)
        {
            if (stream is MemoryStream memoryStream)
            {
                return Convert.ToBase64String(memoryStream.ToArray());
            }

            byte[] bytes = new byte[(int)stream.Length];

            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(bytes, 0, (int)stream.Length);
            return Convert.ToBase64String(bytes);
        }
    }
}
