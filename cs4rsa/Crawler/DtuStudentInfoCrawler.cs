using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using cs4rsa.Helpers;
using cs4rsa.BasicData;
using System.Globalization;
using cs4rsa.Interfaces;

namespace cs4rsa.Crawler
{
    public class DtuStudentInfoCrawler
    {
        public static StudentInfo ToStudentInfo(string specialString, ISaver<StudentInfo> studentSaver)
        {
            string url = $"https://mydtu.duytan.edu.vn/Modules/mentor/WarningDetail.aspx?t={Helpers.Helpers.GetTimeFromEpoch()}&stid={specialString}";
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
            string birthday = StringHelper.SuperCleanString(birthdayNode.InnerText);
            string cmnd = cmndNode.InnerText;
            string email = emailNode.InnerText;
            string phoneNumber = phoneNumberNode.InnerText;
            string address = StringHelper.SuperCleanString(addressNode.InnerText);

            string imageSrcData = imageNode.Attributes["src"].Value;
            string imageBase64Data = imageSrcData.Replace("data:image/jpg;base64,", "");

            StudentInfo info = new StudentInfo()
            {
                Name = name,
                StudentId = studentId,
                SpecialString = specialString,
                Birthday = birthday,
                CMND = cmnd,
                Email = email,
                PhoneNumber = phoneNumber,
                Address = address,
                Image = imageBase64Data
            };
            studentSaver.Save(info);
            return info;
        }
    }
}
