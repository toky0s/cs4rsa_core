﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.Crawler;
using cs4rsa.BasicData;
using NUnit.Framework;

namespace cs4rsa_test.Crawler
{
    [TestFixture]
    class TestHomeCourseSearch
    {
        public HomeCourseSearch hcs;

        [SetUp]
        public void Setup()
        {
            hcs = HomeCourseSearch.GetInstance();
        }

        [Test]
        public void GetCurrentSemesterValue()
        {
            Assert.AreEqual("73", hcs.CurrentSemesterValue);
        }

        [Test]
        public void GetCurrentYearValue()
        {
            Assert.AreEqual("69", hcs.CurrentYearValue);
        }

        [Test]
        public void GetCurrentSemesterInfo()
        {
            Assert.AreEqual("Học Kỳ Hè", hcs.CurrentSemesterInfo);
        }

        [Test]
        public void GetCurrentYearInfo()
        {
            Assert.AreEqual("Năm Học 2020-2021", hcs.CurrentYearInfo);
        }
    }
}