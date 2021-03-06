using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using cs4rsa.Crawler;

namespace cs4rsa.Tests.Crawler
{
    [TestFixture]
    class TestDisciplineData
    {
        public DisciplineData disciplineData;
        [SetUp]
        public void Setup()
        {
            disciplineData = new DisciplineData();
            //disciplineData.DisciplineDictToJsonFile();
        }


        [TestCase]
        public void TestFirstDisciplineKeyword()
        {
            Assert.AreEqual("ACC", disciplineData.GetDisciplines()[0]);
        }

        [TestCase]
        public void TestGetSubjectName()
        {
            Assert.AreEqual("ACC", disciplineData.GetDisciplines()[0]);
        }

        [TestCase]
        public void TestFindKeyword1sFromDiscipline()
        {
            Assert.AreEqual("201", disciplineData.GetKeyword1s("ACC")[0]);
        }

        [TestCase]
        public void TestFindKeyword1sFromDiscipline1()
        {
            Assert.AreEqual("202", disciplineData.GetKeyword1s("ACC")[1]);
        }

        [TestCase]
        public void TestGetSubjectFromDisciplineAndKeyword1()
        {
            DisciplineKeywordInfo disciplineKeywordInfo = disciplineData.GetDisciplineKeywordInfos("ACC")[0];
            Assert.AreEqual("301", disciplineKeywordInfo.CourseID);
            Assert.AreEqual("Nguyên Lý Kế Toán 1", disciplineKeywordInfo.SubjectName);
            Assert.AreEqual("ACC", disciplineKeywordInfo.Discipline);
            Assert.AreEqual("201", disciplineKeywordInfo.Keyword1);
        }
    }
}
