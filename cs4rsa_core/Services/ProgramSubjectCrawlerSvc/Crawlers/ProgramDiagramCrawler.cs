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
            #region Clean các môn chương trình học trong DB
            _unitOfWork.PreProDetails.RemoveRange(await _unitOfWork.PreProDetails.GetAllAsync());
            _unitOfWork.ParProDetails.RemoveRange(await _unitOfWork.ParProDetails.GetAllAsync());
            _unitOfWork.ProgramSubjects.RemoveRange(await _unitOfWork.ProgramSubjects.GetAllAsync());
            _unitOfWork.PreParSubjects.RemoveRange(await _unitOfWork.PreParSubjects.GetAllAsync());
            #endregion

            Student student = await _unitOfWork.Students.GetBySpecialStringAsync(specialString);
            int curid = student.CurriculumId;
            string t = GetTimeFromEpoch();

            StudentProgramCrawler.SetInfor(sessionId, studentId);
            Task<ProgramFolder> task1 = GetNewInstanceStudentProgramCrawler().GetNodeA(specialString, t, VMConstants.NODE_NAME_DAI_CUONG, curid);
            //Task<ProgramFolder> task2 = GetNewInstanceStudentProgramCrawler().GetNode(specialString, t, VMConstants.NODE_NAME_GIAO_DUC_THE_CHAT_VA_QUOC_PHONG, curid);
            //Task<ProgramFolder> task3 = GetNewInstanceStudentProgramCrawler().GetNode(specialString, t, VMConstants.NODE_NAME_DAI_CUONG_NGANH, curid);
            //Task<ProgramFolder> task4 = GetNewInstanceStudentProgramCrawler().GetNode(specialString, t, VMConstants.NODE_NAME_CHUYEN_NGANH, curid);
            await _unitOfWork.CompleteAsync();
            //return await Task.WhenAll(task1, task2, task3, task4);
            return await Task.WhenAll(task1);
        }

        private static StudentProgramCrawler GetNewInstanceStudentProgramCrawler()
        {
            return (StudentProgramCrawler)((App)Application.Current).Container.GetService(typeof(StudentProgramCrawler));
        }
    }
}
