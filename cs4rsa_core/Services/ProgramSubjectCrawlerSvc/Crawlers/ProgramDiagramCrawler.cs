using Cs4rsa.BaseClasses;
using Cs4rsa.Constants;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;
using Cs4rsa.Services.ProgramSubjectCrawlerSvc.DataTypes;

using System.Threading.Tasks;
using System.Windows;

namespace Cs4rsa.Services.ProgramSubjectCrawlerSvc.Crawlers
{
    /// <summary>
    /// Bộ cào này sẽ cào toàn bộ chương trình học của một sinh viên.
    /// </summary>
    public class ProgramDiagramCrawler : BaseCrawler
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProgramDiagramCrawler(
            IUnitOfWork unitOfWork
        )
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ProgramFolder[]> ToProgramDiagram(string sessionId, string specialString, string studentId)
        {
            await Task.WhenAll(
                _unitOfWork.PreProDetails.RemoveAll(),
                _unitOfWork.ParProDetails.RemoveAll(),
                _unitOfWork.ProgramSubjects.RemoveAll(),
                _unitOfWork.PreParSubjects.RemoveAll()
            );

            Student student = await _unitOfWork.Students.GetBySpecialStringAsync(specialString);
            int curid = student.CurriculumId;
            string t = GetTimeFromEpoch();

            StudentProgramCrawler.SetInfor(sessionId, studentId);
            Task<ProgramFolder> task1 = GetNewInstanceStudentProgramCrawler().GetRoot(specialString, t, VMConstants.NODE_NAME_DAI_CUONG, curid);
            Task<ProgramFolder> task2 = GetNewInstanceStudentProgramCrawler().GetRoot(specialString, t, VMConstants.NODE_NAME_GIAO_DUC_THE_CHAT_VA_QUOC_PHONG, curid);
            Task<ProgramFolder> task3 = GetNewInstanceStudentProgramCrawler().GetRoot(specialString, t, VMConstants.NODE_NAME_DAI_CUONG_NGANH, curid);
            Task<ProgramFolder> task4 = GetNewInstanceStudentProgramCrawler().GetRoot(specialString, t, VMConstants.NODE_NAME_CHUYEN_NGANH, curid);
            return await Task.WhenAll(task1, task2, task3, task4);
        }

        private static StudentProgramCrawler GetNewInstanceStudentProgramCrawler()
        {
            return (StudentProgramCrawler)((App)Application.Current).Container.GetService(typeof(StudentProgramCrawler));
        }
    }
}
