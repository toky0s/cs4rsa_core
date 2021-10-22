using CurriculumCrawlerService.Crawlers;
using ProgramSubjectCrawlerService.DataTypes;
using HelperService;
using System.ComponentModel;
using Cs4rsaDatabaseService.DataProviders;
using SubjectCrawlService1.Crawlers.Interfaces;
using System.Threading.Tasks;

namespace ProgramSubjectCrawlerService.Crawlers
{
    /// <summary>
    /// Bộ cào này sẽ cào toàn bộ chương trình học của một sinh viên.
    /// </summary>
    public class ProgramDiagramCrawler
    {
        private string _sessionId;
        private string _specialString;
        private readonly CurriculumCrawler _curriculumCrawler;
        private readonly Cs4rsaDbContext _cs4rsaDbContext;
        private readonly IPreParSubjectCrawler _preParSubjectCrawler;

        public ProgramDiagramCrawler(string sessionId, string specialString,
            CurriculumCrawler curriculumCrawler, Cs4rsaDbContext cs4rsaDbContext, IPreParSubjectCrawler preParSubjectCrawler)
        {
            _sessionId = sessionId;
            _specialString = specialString;
            _curriculumCrawler = curriculumCrawler;
            _cs4rsaDbContext = cs4rsaDbContext;
            _preParSubjectCrawler = preParSubjectCrawler;
        }

        public async Task<ProgramDiagram> ToProgramDiagram()
        {
            int curid = CurriculumCrawler.GetCurriculum(_specialString).CurriculumId;
            string t = HelperService.Helpers.GetTimeFromEpoch();
            string url1 = $"https://mydtu.duytan.edu.vn/Modules/curriculuminportal/ajax/LoadChuongTrinhHocEachPart.aspx?t={t}&studentidnumber={_specialString}&acaLevid=3&curid={curid}&cursectionid=2001";
            string url2 = $"https://mydtu.duytan.edu.vn/Modules/curriculuminportal/ajax/LoadChuongTrinhHocEachPart.aspx?t={t}&studentidnumber={_specialString}&acaLevid=3&curid={curid}&cursectionid=2002";
            string url3 = $"https://mydtu.duytan.edu.vn/Modules/curriculuminportal/ajax/LoadChuongTrinhHocEachPart.aspx?t={t}&studentidnumber={_specialString}&acaLevid=3&curid={curid}&cursectionid=2003";
            string url4 = $"https://mydtu.duytan.edu.vn/Modules/curriculuminportal/ajax/LoadChuongTrinhHocEachPart.aspx?t={t}&studentidnumber={_specialString}&acaLevid=3&curid={curid}&cursectionid=2004";

            StudentProgramCrawler programCrawler1 = new(_sessionId, _cs4rsaDbContext, _preParSubjectCrawler);
            StudentProgramCrawler programCrawler2 = new(_sessionId, _cs4rsaDbContext, _preParSubjectCrawler);
            StudentProgramCrawler programCrawler3 = new(_sessionId, _cs4rsaDbContext, _preParSubjectCrawler);
            StudentProgramCrawler programCrawler4 = new(_sessionId, _cs4rsaDbContext, _preParSubjectCrawler);
            await programCrawler1.GetNode(url1);
            await programCrawler2.GetNode(url2);
            await programCrawler3.GetNode(url3);
            await programCrawler4.GetNode(url4);

            ProgramDiagram diagram = new ProgramDiagram(programCrawler1.Root, programCrawler2.Root,
                                                        programCrawler3.Root, programCrawler4.Root, _cs4rsaDbContext);
            return diagram;
        }
    }
}
