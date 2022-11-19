using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;
using Cs4rsa.Services.CurriculumCrawlerSvc.Crawlers.Interfaces;
using Cs4rsa.Services.ProgramSubjectCrawlerSvc.DataTypes;
using Cs4rsa.Services.SubjectCrawlerSvc.Crawlers.Interfaces;
using Cs4rsa.Utils;

using System.Threading.Tasks;

namespace Cs4rsa.Services.ProgramSubjectCrawlerSvc.Crawlers
{
    /// <summary>
    /// Bộ cào này sẽ cào toàn bộ chương trình học của một sinh viên.
    /// </summary>
    public class ProgramDiagramCrawler
    {
        private readonly string _sessionId;
        private readonly string _specialString;
        private readonly ICurriculumCrawler _curriculumCrawler;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPreParSubjectCrawler _preParSubjectCrawler;

        public ProgramDiagramCrawler(
            string sessionId,
            string specialString,
            ICurriculumCrawler curriculumCrawler,
            IUnitOfWork unitOfWork,
            IPreParSubjectCrawler preParSubjectCrawler)
        {
            _sessionId = sessionId;
            _specialString = specialString;
            _curriculumCrawler = curriculumCrawler;
            _unitOfWork = unitOfWork;
            _preParSubjectCrawler = preParSubjectCrawler;
        }

        public async Task<ProgramFolder[]> ToProgramDiagram()
        {
            #region Clean các môn chương trình học trong DB
            await _unitOfWork.BeginTransAsync();
            _unitOfWork.PreProDetails.RemoveRange(await _unitOfWork.PreProDetails.GetAllAsync());
            _unitOfWork.ParProDetails.RemoveRange(await _unitOfWork.ParProDetails.GetAllAsync());
            _unitOfWork.ProgramSubjects.RemoveRange(await _unitOfWork.ProgramSubjects.GetAllAsync());
            _unitOfWork.PreParSubjects.RemoveRange(await _unitOfWork.PreParSubjects.GetAllAsync());
            #endregion

            Curriculum curriculum = await _curriculumCrawler.GetCurriculum(_specialString);
            int curid = curriculum.CurriculumId;
            string t = Helpers.GetTimeFromEpoch();
            string url1 = LoadChuongTrinhHocEachPart(t, curid, PhanHoc.DAI_CUONG);
            string url2 = LoadChuongTrinhHocEachPart(t, curid, PhanHoc.GIAO_DUC_THE_CHAT_VA_QUOC_PHONG);
            string url3 = LoadChuongTrinhHocEachPart(t, curid, PhanHoc.DAI_CUONG_NGANH);
            string url4 = LoadChuongTrinhHocEachPart(t, curid, PhanHoc.CHUYEN_NGANH);

            StudentProgramCrawler programCrawler1 = new(_sessionId, _unitOfWork, _preParSubjectCrawler);
            StudentProgramCrawler programCrawler2 = new(_sessionId, _unitOfWork, _preParSubjectCrawler);
            StudentProgramCrawler programCrawler3 = new(_sessionId, _unitOfWork, _preParSubjectCrawler);
            StudentProgramCrawler programCrawler4 = new(_sessionId, _unitOfWork, _preParSubjectCrawler);

            Task<ProgramFolder> task1 = programCrawler1.GetNode(url1);
            Task<ProgramFolder> task2 = programCrawler2.GetNode(url2);
            Task<ProgramFolder> task3 = programCrawler3.GetNode(url3);
            Task<ProgramFolder> task4 = programCrawler4.GetNode(url4);

            await _unitOfWork.CompleteAsync();
            await _unitOfWork.CommitAsync();
            return await Task.WhenAll(task1, task2, task3, task4);
        }

        /// <summary>
        /// Lấy ra đường dẫn tới phần học
        /// </summary>
        /// <param name="t">Epoch</param>
        /// <param name="curid">Mã ngành</param>
        /// <param name="cursectionid">PhanHoc</param>
        /// <returns></returns>
        private string LoadChuongTrinhHocEachPart(string t, int curid, PhanHoc phanHoc)
        {
            return $"https://mydtu.duytan.edu.vn/Modules/curriculuminportal/ajax/LoadChuongTrinhHocEachPart.aspx?t={t}&studentidnumber={_specialString}&acaLevid=3&curid={curid}&cursectionid={phanHoc.MaPhanHoc}";
        }

        /// <summary>
        /// Enum chứa mã phần học
        /// </summary>
        private class PhanHoc
        {
            public readonly static PhanHoc DAI_CUONG = new("2001");
            public readonly static PhanHoc GIAO_DUC_THE_CHAT_VA_QUOC_PHONG = new("2002");
            public readonly static PhanHoc DAI_CUONG_NGANH = new("2003");
            public readonly static PhanHoc CHUYEN_NGANH = new("2004");

            public readonly string MaPhanHoc;
            private PhanHoc(string maPhanHoc)
            {
                MaPhanHoc = maPhanHoc;
            }
        }
    }
}
