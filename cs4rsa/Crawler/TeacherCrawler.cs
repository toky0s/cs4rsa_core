using cs4rsa.BasicData;
using cs4rsa.Database;
using cs4rsa.Interfaces;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cs4rsa.Crawler
{
    public class TeacherCrawler
    {
        private Cs4rsaDatabase cs4RsaDatabase;
        private bool SaveDatabase = false;
        private bool IsGetTeacherFromDataBase = false;
        private string intructorId;
        private ISaver<Teacher> teacherSaver;
        public ISaver<Teacher> TeacherSaver
        {
            get
            {
                return teacherSaver;
            }
            set
            {
                SaveDatabase = true;
                teacherSaver = value;
            }
        }

        //http://courses.duytan.edu.vn/Sites/Home_ChuongTrinhDaoTao.aspx?p=home_lecturerdetail&timespan=71&intructorid=010132007&classid=139631&academicleveltypeid=&curriculumid=
        private HtmlDocument document;
        public TeacherCrawler(string url)
        {
            if (url != null)
            {
                intructorId = GetIntructorId(url);
                cs4RsaDatabase = Cs4rsaDatabase.GetInstance();
                if (ThisTeacherIsNotInDatabase(intructorId))
                {
                    HtmlWeb web = new HtmlWeb();
                    document = web.Load(url);
                }
                else
                {
                    IsGetTeacherFromDataBase = true;
                }
            }
        }

        public TeacherCrawler(string semesterValue, string intructorId)
        {
            HomeCourseSearch homeCourseSearch = HomeCourseSearch.GetInstance();
            string url = $@"http://courses.duytan.edu.vn/Sites/Home_ChuongTrinhDaoTao.aspx?p=home_lecturerdetail&timespan={semesterValue}&intructorid={intructorId}";
            HtmlWeb web = new HtmlWeb();
            document = web.Load(url);
        }

        public Teacher ToTeacher()
        {
            Teacher teacher;
            if (document != null)
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
                teacher = new Teacher(id, name, sex, place, degree, workUnit, position, subject, form, teachedSubjects);
                if (SaveDatabase)
                {
                    teacherSaver.Save(teacher);
                }
                return teacher;
            }
            if (IsGetTeacherFromDataBase)
            {
                TeacherDatabase teacherDatabase = new TeacherDatabase(intructorId);
                teacher = teacherDatabase.ToTeacher();
                return teacher;
            }
            return null;
        }

        private string GetIntructorId(string url)
        {
            string[] slideChars = { "&" };
            string[] separatingStrings = { "=" };
            string intructorIdParam = url.Split(slideChars, StringSplitOptions.RemoveEmptyEntries)[2];
            return intructorIdParam.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries)[1];
        }

        private bool ThisTeacherIsNotInDatabase(string instructorId)
        {
            string countQueryString = $@"SELECT count(id) from teacher WHERE id like {instructorId}";
            long result = cs4RsaDatabase.GetScalar<long>(countQueryString);
            if (result == 0)
                return true;
            return false;
        }
    }
}
