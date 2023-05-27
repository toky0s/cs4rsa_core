using Cs4rsa.Cs4rsaDatabase.Implements;

namespace Cs4rsaTest
{
    [TestClass]
    public class TestTeacherRepo
    {
        private TeacherRepository _teacherRepo;

        [TestInitialize]
        public void Setup()
        {
            _teacherRepo = new();
        }

        [TestMethod]
        public void When_ExistByID_Given_Id_Expect_Exist()
        {
            bool result = _teacherRepo.ExistByID(11111005);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void When_ExistByID_Given_Id_Expect_NotExist()
        {
            bool result = _teacherRepo.ExistByID(000888999);
            Assert.IsFalse(result);
        }
    }
}
