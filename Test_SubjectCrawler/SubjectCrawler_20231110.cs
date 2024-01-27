using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cs4rsa.Service.SubjectCrawler.Crawlers;
using Cs4rsa.Service.SubjectCrawler.Crawlers.Interfaces;
using Cs4rsa.Service.SubjectCrawler.DataTypes.Enums;
using HtmlAgilityPack;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Test_SubjectCrawler
{
    [TestFixture]
    public class SubjectCrawler_20231110
    {
        private Mock<ICourseHtmlGetter> _mockCourseHtmlGetter;
        private ISubjectCrawler _subjectCrawler;

        [OneTimeSetUp]
        public void SetUp()
        {
            _mockCourseHtmlGetter = new Mock<ICourseHtmlGetter>(MockBehavior.Strict);
            
            // ACC 201:  Nguyên Lý Kế Toán 1
            // http://courses.duytan.edu.vn/Modules/academicprogram/CourseClassResult.aspx?courseid=301&semesterid=83&timespan=83
            _mockCourseHtmlGetter
                .Setup(courseHtmlGetter => courseHtmlGetter.GetHtmlDocument("301", "83"))
                .Returns(() =>
                {
                    var text = File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources",
                        "20231110_ACC_201.html"));
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(text);
                    return Task.FromResult(htmlDoc);
                });
            
            // ACC 349:  Thi Tốt Nghiệp
            // http://courses.duytan.edu.vn/Modules/academicprogram/CourseClassResult.aspx?courseid=999&semesterid=999&timespan=999
            _mockCourseHtmlGetter
                .Setup(courseHtmlGetter => courseHtmlGetter.GetHtmlDocument("999", "999"))
                .Returns(() =>
                {
                    var text = File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources",
                        "20231110_ACC_349.html"));
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(text);
                    return Task.FromResult(htmlDoc);
                });
            
            // CS 211:  Lập Trình Cơ Sở
            // http://courses.duytan.edu.vn/Modules/academicprogram/CourseClassResult.aspx?courseid=56&semesterid=83&timespan=83
            _mockCourseHtmlGetter
                .Setup(courseHtmlGetter => courseHtmlGetter.GetHtmlDocument("56", "83"))
                .Returns(() =>
                {
                    var text = File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources",
                        "20231110_CS_211.html"));
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(text);
                    return Task.FromResult(htmlDoc);
                });
            
            // NULL 99999
            _mockCourseHtmlGetter
                .Setup(courseHtmlGetter => courseHtmlGetter.GetHtmlDocument("99999", "99999"))
                .Returns(() =>
                {
                    var text = File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources",
                        "20231110_NULL_99999.html"));
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(text);
                    return Task.FromResult(htmlDoc);
                });
            
            // CS 252:  Mạng Máy Tính
            // https://courses.duytan.edu.vn/Sites/Home_ChuongTrinhDaoTao.aspx?p=home_listcoursedetail&courseid=65&timespan=75&t=s
            // https://courses.duytan.edu.vn/Modules/academicprogram/CourseClassResult.aspx?courseid=65&semesterid=75&timespan=75
            _mockCourseHtmlGetter
                .Setup(courseHtmlGetter => courseHtmlGetter.GetHtmlDocument("65", "75"))
                .Returns(() =>
                {
                    var text = File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "SubjectHtmls",
                        "20231110_CS_252.html"));
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(text);
                    return Task.FromResult(htmlDoc);
                });
            _subjectCrawler = new SubjectCrawler(_mockCourseHtmlGetter.Object);
        }
        

        [Test] public async Task GivenCourseIdAndSemesterId_WhenCrawl_ThenAreEqualSubjectName()
        {
            var subject = await _subjectCrawler.Crawl("301", "83");
            Assert.Equals("Nguyên Lý Kế Toán 1", subject.Name);
        }
        
        [Test] public async Task GivenCourseIdAndSemesterId_WhenCrawl_ThenAreEqualSubjectCode()
        {
            var subject = await _subjectCrawler.Crawl("301", "83");
            Assert.Equals("ACC 201", subject.SubjectCode);
        }

        [Test] public async Task GivenCourseIdAndSemesterId_WhenCrawl_ThenAreEqualStudyUnit()
        {
            var subject = await _subjectCrawler.Crawl("301", "83");
            Assert.Equals(3, subject.StudyUnit);
        }
        
        [Test] public async Task GivenCourseIdAndSemesterId_WhenCrawl_ThenAreEqualStudyUnitType()
        {
            var subject = await _subjectCrawler.Crawl("301", "83");
            Assert.Equals("Tín Chỉ", subject.StudyUnitType);
        }
        
        [Test] public async Task GivenCourseIdAndSemesterId_WhenCrawl_ThenAreEqualStudyType()
        {
            var subject = await _subjectCrawler.Crawl("301", "83");
            Assert.Equals("LEC", subject.StudyType);
        }
        
        [Test] public async Task GivenCourseIdAndSemesterId_WhenCrawl_ThenAreEqualSemester()
        {
            var subject = await _subjectCrawler.Crawl("301", "83");
            Assert.Equals("Học Kỳ I", subject.Semester);
        }
        
        [Test] public async Task GivenCourseIdAndSemesterId_WhenMustStudySubject_ThenIsEmpty()
        {
            var subject = await _subjectCrawler.Crawl("301", "83");
            CollectionAssert.IsEmpty(subject.MustStudySubject);
        }
        
        [Test] public async Task GivenCourseIdAndSemesterId_WhenParallelSubject_ThenIsEmpty()
        {
            var subject = await _subjectCrawler.Crawl("301", "83");
            CollectionAssert.IsEmpty(subject.ParallelSubject);
        }
        
        [Test] public async Task GivenCourseIdAndSemesterId_WhenDescription_ThenIsEmpty()
        {
            var subject = await _subjectCrawler.Crawl("301", "83");
            CollectionAssert.IsEmpty(subject.Description);
        }
        
        [Test] public async Task GivenCourseIdAndSemesterId_WhenCountClassGroup_ThenEquals()
        {
            var subject = await _subjectCrawler.Crawl("301", "83");
            Assert.Equals(31, subject.ClassGroups.Count);
        }
        
        [Test] public async Task GivenCourseIdAndSemesterId_WhenNotExists_ThenTrue()
        {
            var subject = await _subjectCrawler.Crawl("99999", "99999");
            ClassicAssert.IsNull(subject);
        }
        
        [Test] public async Task GivenCourseIdAndSemesterId_WhenFirstClassGroup_ThenClassGroupName()
        {
            var subject = await _subjectCrawler.Crawl("301", "83");
            Assert.Equals("ACC 201 A", subject.ClassGroups.First().Name);
        }
        
        [Test] public async Task GivenCourseIdAndSemesterId_WhenFirstClassGroup_ThenSubjectName()
        {
            var subject = await _subjectCrawler.Crawl("301", "83");
            Assert.Equals("Nguyên Lý Kế Toán 1", subject.ClassGroups.First().SubjectName);
        }
        
        [Test] public async Task GivenCourseIdAndSemesterId_WhenFirstClassGroup_ThenRegisterCodeCount()
        {
            var subject = await _subjectCrawler.Crawl("301", "83");
            Assert.Equals(1, subject.ClassGroups.First().RegisterCodes.Count);
        }
        
        [Test] public async Task GivenCourseIdAndSemesterId_WhenFirstClassGroup_ThenRegisterCode()
        {
            var subject = await _subjectCrawler.Crawl("301", "83");
            Assert.Equals("ACC201202301001", subject.ClassGroups.First().RegisterCodes.First());
        }
        
        [Test] public async Task GivenCourseIdAndSemesterId_WhenFirstClassGroup_ThenStudyTimeStart()
        {
            var subject = await _subjectCrawler.Crawl("301", "83");
            Assert.Equals(9, subject.ClassGroups.First().SchoolClasses.First().Schedule.ScheduleTime[DayOfWeek.Monday].First().Start.Hour);
            Assert.Equals(15, subject.ClassGroups.First().SchoolClasses.First().Schedule.ScheduleTime[DayOfWeek.Monday].First().Start.Minute);
        }
        
        [Test] public async Task GivenCourseIdAndSemesterId_WhenFirstClassGroup_ThenStudyTimeEnd()
        {
            var subject = await _subjectCrawler.Crawl("301", "83");
            Assert.Equals(11, subject.ClassGroups.First().SchoolClasses.First().Schedule.ScheduleTime[DayOfWeek.Monday].First().End.Hour);
            Assert.Equals(15, subject.ClassGroups.First().SchoolClasses.First().Schedule.ScheduleTime[DayOfWeek.Monday].First().End.Minute);
        }
        
        [Test] public async Task GivenCourseIdAndSemesterId_WhenFirstClassGroup_ThenStudyTimeStartAsString()
        {
            var subject = await _subjectCrawler.Crawl("301", "83");
            Assert.Equals("09:15", subject.ClassGroups.First().SchoolClasses.First().Schedule.ScheduleTime[DayOfWeek.Monday].First().StartAsString);
        }
        
        [Test] public async Task GivenCourseIdAndSemesterId_WhenFirstClassGroup_ThenStudyTimeEndAsString()
        {
            var subject = await _subjectCrawler.Crawl("301", "83");
            Assert.Equals("11:15", subject.ClassGroups.First().SchoolClasses.First().Schedule.ScheduleTime[DayOfWeek.Monday].First().EndAsString);
        }
        
        [Test] public async Task GivenCourseIdAndSemesterId_WhenFirstClassGroup_ThenSession()
        {
            var subject = await _subjectCrawler.Crawl("301", "83");
            Assert.Equals(Session.Morning, subject.ClassGroups.First().SchoolClasses.First().Schedule.ScheduleTime[DayOfWeek.Monday].First().Session);
        }
        
        [Test] public async Task GivenCourseIdAndSemesterId_WhenCrawl_ThenSubject()
        {
            var subject = await _subjectCrawler.Crawl("999", "999");
            Assert.Equals("Thi Tốt Nghiệp", subject.Name);
        }
        
        [Test] public async Task GivenCourseIdAndSemesterId_WhenClassGroups_ThenEmpty()
        {
            var subject = await _subjectCrawler.Crawl("999", "999");
            ClassicAssert.IsEmpty(subject.ClassGroups);
        }
        
        [Test] public async Task GivenCourseIdAndSemesterId_WhenTempTeachers_ThenEmpty()
        {
            var subject = await _subjectCrawler.Crawl("56", "83");
            var tempTeachersCount = subject
                .ClassGroups
                .First(clg => clg.Name == "CS 211 CIS")
                .SchoolClasses
                .First(sc => sc.SchoolClassName == "CS 211 CIS1")
                .TeacherNames
                .Count;
            Assert.Equals(2,tempTeachersCount); 
        }
    }
}