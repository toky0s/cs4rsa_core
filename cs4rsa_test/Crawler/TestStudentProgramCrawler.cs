using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.BasicData;
using cs4rsa.Crawler;
using HtmlAgilityPack;
using NUnit.Framework;
using cs4rsa_test.Crawler;

namespace cs4rsa_test.Crawler
{
    [TestFixture]
    class TestStudentProgramCrawler
    {
        private string url = "https://mydtu.duytan.edu.vn/Modules/curriculuminportal/ajax/LoadChuongTrinhHocEachPart.aspx?t=1623769137415&studentidnumber=ppxdPtQCkOX2+rc5tqBFhg%3D%3D&acaLevid=3&curid=605&cursectionid=2001";
        private string url2 = "https://mydtu.duytan.edu.vn/Modules/curriculuminportal/ajax/LoadChuongTrinhHocEachPart.aspx?t=1623769137415&studentidnumber=ppxdPtQCkOX2+rc5tqBFhg%3D%3D&acaLevid=3&curid=605&cursectionid=2002";
        [Test]
            public void GetTrTags()
        {
            StudentProgramCrawler studentProgramCrawler = new StudentProgramCrawler(InfoForTest.sessionId, url);
            List<HtmlNode> trtags = studentProgramCrawler.GetAllTrTag(url);
            Assert.AreEqual(52, trtags.Count());
        }

        [Test]
        public void GetFolderNodes()
        {
            StudentProgramCrawler studentProgramCrawler = new StudentProgramCrawler(InfoForTest.sessionId, url);
            Assert.AreEqual(13, studentProgramCrawler._folderNodes.Count);
        }

        [Test]
        public void GetFileNodes()
        {
            StudentProgramCrawler studentProgramCrawler = new StudentProgramCrawler(InfoForTest.sessionId, url);
            Assert.AreEqual(39, studentProgramCrawler._fileNodes.Count);
        }

        [Test]
        public void GetProgramFolders()
        {
            StudentProgramCrawler studentProgramCrawler = new StudentProgramCrawler(InfoForTest.sessionId, url);
            Assert.AreEqual(13, studentProgramCrawler.ProgramFolders.Count);
        }

        [Test]
        public void GetProgramSubjects()
        {
            StudentProgramCrawler studentProgramCrawler = new StudentProgramCrawler(InfoForTest.sessionId, url);
            List<ProgramSubject> programSubjects = studentProgramCrawler.GetProgramSubjects(studentProgramCrawler._fileNodes);
            ProgramSubject subject = programSubjects[0];
            Assert.AreEqual("PHI 100", subject.SubjectCode);
            Assert.AreEqual(39, programSubjects.Count);
        }

        [Test]
        public void GetRootFolder()
        {
            StudentProgramCrawler studentProgramCrawler = new StudentProgramCrawler(InfoForTest.sessionId, url);
            Assert.AreEqual("ĐẠI CƯƠNG", studentProgramCrawler.Root.Name);
            Assert.AreEqual(StudyMode.Compulsory, studentProgramCrawler.Root.StudyMode);
        }

        [Test]
        public void GetRootFolder2()
        {
            StudentProgramCrawler studentProgramCrawler = new StudentProgramCrawler(InfoForTest.sessionId, url);
            Assert.AreEqual("Ngoại Ngữ", studentProgramCrawler.Root.ChildProgramFolders[0].Name);
            Assert.AreEqual(false, studentProgramCrawler.Root.ChildProgramFolders[0].IsCompleted());
        }

        [Test]
        public void GetRootFolder3()
        {
            StudentProgramCrawler studentProgramCrawler = new StudentProgramCrawler(InfoForTest.sessionId, url);
            Assert.AreEqual("Khoa Học Xã Hội", studentProgramCrawler.Root.ChildProgramFolders[2].Name);
            Assert.AreEqual(true, studentProgramCrawler.Root.ChildProgramFolders[2].IsCompleted());
        }

        [Test]
        public void GetRootFolder4()
        {
            StudentProgramCrawler studentProgramCrawler = new StudentProgramCrawler(InfoForTest.sessionId, url);
            Assert.AreEqual("Khoa Học Tự Nhiên", studentProgramCrawler.Root.ChildProgramFolders[3].Name);
            Assert.AreEqual(true, studentProgramCrawler.Root.ChildProgramFolders[3].IsCompleted());
        }

        [Test]
        public void GetRootFolder5()
        {
            StudentProgramCrawler studentProgramCrawler = new StudentProgramCrawler(InfoForTest.sessionId, url2);
            Assert.AreEqual("Giáo Dục Thể Chất Cao Cấp (Tự chọn)", studentProgramCrawler.Root.ChildProgramFolders[1].Name);
            Assert.AreEqual(true, studentProgramCrawler.Root.ChildProgramFolders[1].IsCompleted());
        }
    }
}
