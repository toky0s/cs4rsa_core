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

        public ProgramDiagramCrawler(string sessionId, string specialString,
            ICurriculumCrawler curriculumCrawler, IUnitOfWork unitOfWork, IPreParSubjectCrawler preParSubjectCrawler)
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
            Curriculum curriculum = await _curriculumCrawler.GetCurriculum(_specialString);
            int curid = curriculum.CurriculumId;
            string t = Helpers.GetTimeFromEpoch();
            string url1 = $"https://mydtu.duytan.edu.vn/Modules/curriculuminportal/ajax/LoadChuongTrinhHocEachPart.aspx?t={t}&studentidnumber={_specialString}&acaLevid=3&curid={curid}&cursectionid=2001";
            string url2 = $"https://mydtu.duytan.edu.vn/Modules/curriculuminportal/ajax/LoadChuongTrinhHocEachPart.aspx?t={t}&studentidnumber={_specialString}&acaLevid=3&curid={curid}&cursectionid=2002";
            string url3 = $"https://mydtu.duytan.edu.vn/Modules/curriculuminportal/ajax/LoadChuongTrinhHocEachPart.aspx?t={t}&studentidnumber={_specialString}&acaLevid=3&curid={curid}&cursectionid=2003";
            string url4 = $"https://mydtu.duytan.edu.vn/Modules/curriculuminportal/ajax/LoadChuongTrinhHocEachPart.aspx?t={t}&studentidnumber={_specialString}&acaLevid=3&curid={curid}&cursectionid=2004";

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
    }
}
