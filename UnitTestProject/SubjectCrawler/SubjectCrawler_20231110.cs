using Cs4rsa.Service.SubjectCrawler.Crawlers;
using Cs4rsa.Service.SubjectCrawler.Crawlers.Interfaces;
using Cs4rsa.Service.SubjectCrawler.DataTypes.Enums;

using HtmlAgilityPack;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NSubstitute;

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Test_SubjectCrawler
{
    [TestClass]
    
    public class SubjectCrawler_20231110
    {
        private ICourseHtmlGetter _mockCourseHtmlGetter; 
        private ISubjectCrawler _subjectCrawler;
        private string _baseDir;

        [TestInitialize]
        
        public void SetUp()
        {
            _baseDir = AppDomain.CurrentDomain.BaseDirectory; 
            _mockCourseHtmlGetter = Substitute.For<ICourseHtmlGetter>(); 
            
            var folder = "SubjectCrawler/Resources";
            // ACC 201: Nguyên Lý Kế Toán 1
            _mockCourseHtmlGetter.GetHtmlDocument("301", "83")
                .Returns(x => Task.FromResult(LoadHtmlFromResource(folder, "20231110_ACC_201.html"))); 
            
            // ACC 349: Thi Tốt Nghiệp
            _mockCourseHtmlGetter.GetHtmlDocument("999", "999")
                .Returns(x => Task.FromResult(LoadHtmlFromResource(folder, "20231110_ACC_349.html"))); 
            
            // CS 211: Lập Trình Cơ Sở
            _mockCourseHtmlGetter.GetHtmlDocument("56", "83")
                .Returns(x => Task.FromResult(LoadHtmlFromResource(folder, "20231110_CS_211.html"))); 
            
            // NULL 99999
            _mockCourseHtmlGetter.GetHtmlDocument("99999", "99999")
                .Returns(x => Task.FromResult(LoadHtmlFromResource(folder, "20231110_NULL_99999.html"))); 
            
            // CS 252: Mạng Máy Tính
            _mockCourseHtmlGetter.GetHtmlDocument("65", "75")
                .Returns(x => Task.FromResult(LoadHtmlFromResource(folder, "20231110_CS_252.html"))); 

            _subjectCrawler = new SubjectCrawler(_mockCourseHtmlGetter); 
        }

        private HtmlDocument LoadHtmlFromResource(string folder, string fileName)
        {
            var text = File.ReadAllText(Path.Combine(_baseDir, folder, fileName)); 
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(text);
            return htmlDoc;
        }

        [TestMethod]
        
        public async Task GivenCourseIdAndSemesterId_WhenCrawl_ThenAreEqualSubjectName()
        {
            var subject = await _subjectCrawler.Crawl("301", "83");
            Assert.AreEqual("Nguyên Lý Kế Toán 1", subject.Item1.Name); 
        }

        [TestMethod]
        
        public async Task GivenCourseIdAndSemesterId_WhenCrawl_ThenAreEqualSubjectCode()
        {
            var subject = await _subjectCrawler.Crawl("301", "83");
            Assert.AreEqual("ACC 201", subject.Item1.SubjectCode); 
        }

        [TestMethod]
        
        public async Task GivenCourseIdAndSemesterId_WhenCrawl_ThenAreEqualStudyUnit()
        {
            var subject = await _subjectCrawler.Crawl("301", "83");
            Assert.AreEqual(3, subject.Item1.StudyUnit); 
        }

        [TestMethod]
        
        public async Task GivenCourseIdAndSemesterId_WhenCrawl_ThenAreEqualStudyUnitType()
        {
            var subject = await _subjectCrawler.Crawl("301", "83");
            Assert.AreEqual("Tín Chỉ", subject.Item1.StudyUnitType); 
        }

        [TestMethod]
        
        public async Task GivenCourseIdAndSemesterId_WhenCrawl_ThenAreEqualStudyType()
        {
            var subject = await _subjectCrawler.Crawl("301", "83");
            Assert.AreEqual("LEC", subject.Item1.StudyType); 
        }

        [TestMethod]
        
        public async Task GivenCourseIdAndSemesterId_WhenCrawl_ThenAreEqualSemester()
        {
            var subject = await _subjectCrawler.Crawl("301", "83");
            Assert.AreEqual("Học Kỳ I", subject.Item1.Semester); 
        }

        [TestMethod]
        
        public async Task GivenCourseIdAndSemesterId_WhenMustStudySubject_ThenIsEmpty()
        {
            var subject = await _subjectCrawler.Crawl("301", "83");
            Assert.IsFalse(subject.Item1.MustStudySubject.Any()); 
        }

        [TestMethod]
        
        public async Task GivenCourseIdAndSemesterId_WhenParallelSubject_ThenIsEmpty()
        {
            var subject = await _subjectCrawler.Crawl("301", "83");
            Assert.IsFalse(subject.Item1.ParallelSubject.Any()); 
        }

        [TestMethod]
        
        public async Task GivenCourseIdAndSemesterId_WhenDescription_ThenIsEmpty()
        {
            var subject = await _subjectCrawler.Crawl("301", "83");
            Assert.IsFalse(subject.Item1.Description.Any()); 
        }

        [TestMethod]
        
        public async Task GivenCourseIdAndSemesterId_WhenCountClassGroup_ThenEquals()
        {
            var subject = await _subjectCrawler.Crawl("301", "83");
            Assert.AreEqual(31, subject.Item1.ClassGroups.Count); 
        }

        [TestMethod]
        
        public async Task GivenCourseIdAndSemesterId_WhenNotExists_ThenTrue()
        {
            var subject = await _subjectCrawler.Crawl("99999", "99999");
            Assert.IsNull(subject.Item1); 
        }

        [TestMethod]
        
        public async Task GivenCourseIdAndSemesterId_WhenFirstClassGroup_ThenClassGroupName()
        {
            var subject = await _subjectCrawler.Crawl("301", "83");
            Assert.AreEqual("ACC 201 A", subject.Item1.ClassGroups.First().Name); 
        }

        [TestMethod]
        
        public async Task GivenCourseIdAndSemesterId_WhenFirstClassGroup_ThenSubjectName()
        {
            var subject = await _subjectCrawler.Crawl("301", "83");
            Assert.AreEqual("Nguyên Lý Kế Toán 1", subject.Item1.ClassGroups.First().SubjectName); 
        }

        [TestMethod]
        
        public async Task GivenCourseIdAndSemesterId_WhenFirstClassGroup_ThenRegisterCodeCount()
        {
            var subject = await _subjectCrawler.Crawl("301", "83");
            Assert.AreEqual(1, subject.Item1.ClassGroups.First().RegisterCodes.Count); 
        }

        [TestMethod]
        
        public async Task GivenCourseIdAndSemesterId_WhenFirstClassGroup_ThenRegisterCode()
        {
            var subject = await _subjectCrawler.Crawl("301", "83");
            Assert.AreEqual("ACC201202301001", subject.Item1.ClassGroups.First().RegisterCodes.First()); 
        }

        [TestMethod]
        
        public async Task GivenCourseIdAndSemesterId_WhenFirstClassGroup_ThenStudyTimeStart()
        {
            var subject = await _subjectCrawler.Crawl("301", "83");
            var firstSchedule = subject.Item1.ClassGroups.First().SchoolClasses.First().Schedule.ScheduleTime[DayOfWeek.Monday].First();
            Assert.AreEqual(9, firstSchedule.Start.Hour); 
            Assert.AreEqual(15, firstSchedule.Start.Minute); 
        }

        [TestMethod]
        
        public async Task GivenCourseIdAndSemesterId_WhenFirstClassGroup_ThenStudyTimeEnd()
        {
            var subject = await _subjectCrawler.Crawl("301", "83");
            var firstSchedule = subject.Item1.ClassGroups.First().SchoolClasses.First().Schedule.ScheduleTime[DayOfWeek.Monday].First();
            Assert.AreEqual(11, firstSchedule.End.Hour); 
            Assert.AreEqual(15, firstSchedule.End.Minute); 
        }

        [TestMethod]
        
        public async Task GivenCourseIdAndSemesterId_WhenFirstClassGroup_ThenStudyTimeStartAsString()
        {
            var subject = await _subjectCrawler.Crawl("301", "83");
            Assert.AreEqual("09:15", subject.Item1.ClassGroups.First().SchoolClasses.First().Schedule.ScheduleTime[DayOfWeek.Monday].First().StartAsString); 
        }

        [TestMethod]
        
        public async Task GivenCourseIdAndSemesterId_WhenFirstClassGroup_ThenStudyTimeEndAsString()
        {
            var subject = await _subjectCrawler.Crawl("301", "83");
            Assert.AreEqual("11:15", subject.Item1.ClassGroups.First().SchoolClasses.First().Schedule.ScheduleTime[DayOfWeek.Monday].First().EndAsString); 
        }

        [TestMethod]
        
        public async Task GivenCourseIdAndSemesterId_WhenFirstClassGroup_ThenSession()
        {
            var subject = await _subjectCrawler.Crawl("301", "83");
            Assert.AreEqual(Session.Morning, subject.Item1.ClassGroups.First().SchoolClasses.First().Schedule.ScheduleTime[DayOfWeek.Monday].First().Session); 
        }

        [TestMethod]
        
        public async Task GivenCourseIdAndSemesterId_WhenClassGroups_ThenEmpty()
        {
            var ex = await Assert.ThrowsExceptionAsync<IndexOutOfRangeException>(async () =>
            {
                var subject2 = await _subjectCrawler.Crawl("999", "999");
                var classGroupsCount = subject2.Item1.ClassGroups.Count;
            });

            Assert.AreEqual("Không tồn tại bảng lịch", ex.Message);
        }

        [TestMethod]
        
        public async Task GivenCourseIdAndSemesterId_WhenTempTeachers_ThenEmpty()
        {
            var subject = await _subjectCrawler.Crawl("56", "83");
            var tempTeachersCount = subject.Item1
                .ClassGroups
                .First(clg => clg.Name == "CS 211 CIS")
                .SchoolClasses
                .First(sc => sc.SchoolClassName == "CS 211 CIS1")
                .TeacherNames
                .Count;
            Assert.AreEqual(2, tempTeachersCount); 
        }
    }
}