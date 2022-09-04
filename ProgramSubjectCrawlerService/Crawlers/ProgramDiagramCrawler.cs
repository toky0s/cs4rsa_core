using Cs4rsaDatabaseService.Interfaces;
using Cs4rsaDatabaseService.Models;

using CurriculumCrawlerService.Crawlers.Interfaces;

using HelperService;

using ProgramSubjectCrawlerService.DataTypes;

using SubjectCrawlService1.Crawlers.Interfaces;

using System;
using System.Threading.Tasks;

namespace ProgramSubjectCrawlerService.Crawlers
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

        public Func<ProgramFolder, Task> AddProgramFolder { get; set; }
        public async Task ToProgramDiagram()
        {
            #region Clean các môn chương trình học trong DB
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

            ProgramFolder task1 = await programCrawler1.GetNode(url1);
            await AddProgramFolder.Invoke(task1);

            ProgramFolder task2 = await programCrawler2.GetNode(url2);
            await AddProgramFolder.Invoke(task2);

            ProgramFolder task3 = await programCrawler3.GetNode(url3);
            await AddProgramFolder.Invoke(task3);

            ProgramFolder task4 = await programCrawler4.GetNode(url4);
            await AddProgramFolder.Invoke(task4);
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
