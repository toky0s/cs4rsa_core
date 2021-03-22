using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NUnit.Framework;
using cs4rsa.Database;
using cs4rsa.Crawler;

namespace cs4rsa.Tests.Database
{
    class TestDataDisciplineKeyword
    {
        [Test]
        public void TestCreateDataBase()
        {
            Cs4rsaData cs4RsaData = new Cs4rsaData();
            Assert.AreEqual(true, File.Exists("cs4rsadb.db"));
        }

        [Test]
        public void TestCrawlDataToDatabase()
        {
            DisciplineData disciplineData = new DisciplineData();
            disciplineData.GetDisciplineAndKeywordDatabase();
        }
    }
}
