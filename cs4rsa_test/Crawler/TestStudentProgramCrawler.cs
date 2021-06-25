using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.BasicData;
using cs4rsa.Crawler;
using HtmlAgilityPack;
using NUnit.Framework;

namespace cs4rsa_test.Crawler
{
    [TestFixture]
    class TestStudentProgramCrawler
    {
        private string sessionId = "";
        [Test]
        public void GetTrTags()
        {
            StudentProgramCrawler studentProgramCrawler = new StudentProgramCrawler("ppxdPtQCkOX2 rc5tqBFhg==",sessionId);
            List<HtmlNode> trtags = studentProgramCrawler.GetAllTrTag();
            Assert.AreEqual(52, trtags.Count());
        }

        [Test]
        public void GetFolderNodes()
        {
            StudentProgramCrawler studentProgramCrawler = new StudentProgramCrawler("ppxdPtQCkOX2 rc5tqBFhg==", sessionId);
            Assert.AreEqual(13, studentProgramCrawler._folderNodes.Count);
        }

        [Test]
        public void GetFileNodes()
        {
            StudentProgramCrawler studentProgramCrawler = new StudentProgramCrawler("ppxdPtQCkOX2 rc5tqBFhg==", sessionId);
            Assert.AreEqual(39, studentProgramCrawler._fileNodes.Count);
        }

        [Test]
        public void GetProgramFolder()
        {
            StudentProgramCrawler studentProgramCrawler = new StudentProgramCrawler("ppxdPtQCkOX2 rc5tqBFhg==", sessionId);
            List<ProgramFolder> programFolders = studentProgramCrawler.GetProgramFolders(studentProgramCrawler._folderNodes);
        }

        [Test]
        public void GetProgramSubjects()
        {
            StudentProgramCrawler studentProgramCrawler = new StudentProgramCrawler("ppxdPtQCkOX2 rc5tqBFhg==", sessionId);
            List<ProgramSubject> programFolders = studentProgramCrawler.GetProgramSubjects(studentProgramCrawler._fileNodes);
        }
    }
}
