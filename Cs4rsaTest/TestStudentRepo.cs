using Cs4rsa.Cs4rsaDatabase.Implements;
using Cs4rsa.Cs4rsaDatabase.Models;

namespace Cs4rsaTest
{
    [TestClass]
    public class TestStudentRepo
    {
        private StudentRepository _studentRepository;

        [TestInitialize]
        public void Setup()
        {
            _studentRepository = new StudentRepository();
        }

        [TestMethod]
        public void When_CountByContainsId_Given_NonExistId_Expect_Zero()
        {
            long result = _studentRepository.CountByContainsId("242052054931231");
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void When_CountByContainsId_Given_ExistId_Expect_Positive()
        {
            long result = _studentRepository.CountByContainsId("24205205493");
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void When_CountByContainsId_Given_ExistId_Expect_Positive2()
        {
            long result = _studentRepository.CountByContainsId("26204335764");
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void When_ExistsBySpecialString_Given_ExistSpecialString_Expect_True()
        {
            bool result = _studentRepository.ExistsBySpecialString("miH4p+yECKvNwzWcywSlQQ==");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void When_ExistsBySpecialString_Given_ExistSpecialString_Expect_False()
        {
            bool result = _studentRepository.ExistsBySpecialString("xxxxxXXXXX12345679");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void When_GetAllBySpecialStringNotNull_Expect_TwoRecord()
        {
            IEnumerable<Student> students = _studentRepository.GetAllBySpecialStringNotNull();
            Assert.AreEqual(2, students.Count());
        }

        [TestMethod]
        public void When_GetAllBySpecialStringNotNull_Expect_ValidInfor()
        {
            IEnumerable<Student> students = _studentRepository.GetAllBySpecialStringNotNull();
            Student student = students.ElementAt(0);
            Assert.AreEqual("Nguyễn Trần Thanh Tâm", student.Name);
        }

        [TestMethod]
        public void When_GetByStudentId_Given_ExistID_Expect_ValidInfor()
        {
            Student student = _studentRepository.GetByStudentId("24205205493");
            Assert.AreEqual("24205205493", student.StudentId);
            Assert.AreEqual("7lOQDrVSAcaBZ3e6nJXYRQ==", student.SpecialString);
            Assert.AreEqual(DateTime.Parse("2000-08-26 00:00:00"), student.BirthDay);
            Assert.AreEqual("215518087", student.Cmnd);
            Assert.AreEqual("nguyentthanhtam33@dtu.edu.vn", student.Email);
            Assert.AreEqual("7515243", student.PhoneNumber);
            Assert.AreEqual("tấn thạnh I, P. Hoài Hảo, Q. 129, 8, Việt Nam", student.Address);
            Assert.AreEqual("C:\\Users\\truon\\Source\\Repos\\cs4rsa_core\\cs4rsa_core\\bin\\Debug\\net6.0-windows\\StudentImages\\24205205493.jpg", student.AvatarImgPath);
            Assert.AreEqual(629, student.CurriculumId);
        }

        [TestMethod]
        public void When_GetByStudentId_Given_NotExistID_Expect_Null()
        {
            Student student = _studentRepository.GetByStudentId("00009999988888");
            Assert.IsNull(student);
        }

        [TestMethod]
        public void When_GetStudentsByContainsId_Given_StudentID_Expect_Count()
        {
            IEnumerable<Student> students = _studentRepository.GetStudentsByContainsId("24", 1, 0);
            Assert.AreEqual(1, students.Count());
        }

        [TestMethod]
        public void When_GetStudentsByContainsId_Given_StudentID_Expect_ValidInfor()
        {
            IEnumerable<Student> students = _studentRepository.GetStudentsByContainsId("24", 1, 0);
            var student = students.ElementAt(0);
            Assert.AreEqual("24205205493", student.StudentId);
            Assert.AreEqual("7lOQDrVSAcaBZ3e6nJXYRQ==", student.SpecialString);
            Assert.AreEqual(DateTime.Parse("2000-08-26 00:00:00"), student.BirthDay);
            Assert.AreEqual("215518087", student.Cmnd);
            Assert.AreEqual("nguyentthanhtam33@dtu.edu.vn", student.Email);
            Assert.AreEqual("7515243", student.PhoneNumber);
            Assert.AreEqual("tấn thạnh I, P. Hoài Hảo, Q. 129, 8, Việt Nam", student.Address);
            Assert.AreEqual("C:\\Users\\truon\\Source\\Repos\\cs4rsa_core\\cs4rsa_core\\bin\\Debug\\net6.0-windows\\StudentImages\\24205205493.jpg", student.AvatarImgPath);
            Assert.AreEqual(629, student.CurriculumId);
        }
    }
}
