using Cs4rsa.Cs4rsaDatabase.Implements;

namespace Cs4rsaTest
{
    [TestClass]
    public class TestProgramSubjectRepository
    {
        private ProgramSubjectRepository _programSubjectRepository;

        [TestInitialize]
        public void Setup()
        {
            _programSubjectRepository = new ProgramSubjectRepository();
        }

        [TestMethod]
        public void When_ExistsByCourseId_Expect_Exist()
        {
            bool result = _programSubjectRepository.ExistsByCourseId("1");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void When_ExistsByCourseId_Expect_Not_Exist()
        {
            bool result = _programSubjectRepository.ExistsByCourseId("12314121");
            Assert.IsFalse(result);
        }
    }
}
