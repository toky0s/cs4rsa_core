using cs4rsa.BasicData;
using cs4rsa.Implements;
using cs4rsa.Interfaces;
using System.ComponentModel;

namespace cs4rsa.Crawler
{
    /// <summary>
    /// Bộ cào này sẽ cào toàn bộ chương trình học của một sinh viên.
    /// </summary>
    class ProgramDiagramCrawler
    {
        private string _sessionId;
        private string _specialString;
        private BackgroundWorker _backgroundWorker;

        public ProgramDiagramCrawler(string sessionId, string specialString, BackgroundWorker backgroundWorker = null)
        {
            _sessionId = sessionId;
            _specialString = specialString;
            _backgroundWorker = backgroundWorker;
        }

        public ProgramDiagram ToProgramDiagram()
        {
            string curid = CurriculumCrawler.GetCurriculum(_specialString).CurId;
            string t = Helpers.Helpers.GetTimeFromEpoch();
            string url1 = $"https://mydtu.duytan.edu.vn/Modules/curriculuminportal/ajax/LoadChuongTrinhHocEachPart.aspx?t={t}&studentidnumber={_specialString}&acaLevid=3&curid={curid}&cursectionid=2001";
            string url2 = $"https://mydtu.duytan.edu.vn/Modules/curriculuminportal/ajax/LoadChuongTrinhHocEachPart.aspx?t={t}&studentidnumber={_specialString}&acaLevid=3&curid={curid}&cursectionid=2002";
            string url3 = $"https://mydtu.duytan.edu.vn/Modules/curriculuminportal/ajax/LoadChuongTrinhHocEachPart.aspx?t={t}&studentidnumber={_specialString}&acaLevid=3&curid={curid}&cursectionid=2003";
            string url4 = $"https://mydtu.duytan.edu.vn/Modules/curriculuminportal/ajax/LoadChuongTrinhHocEachPart.aspx?t={t}&studentidnumber={_specialString}&acaLevid=3&curid={curid}&cursectionid=2004";

            PreParGetter preParGetter = new PreParGetter();
            ProgramSubjectSaver programSubjectSaver = new ProgramSubjectSaver();
            StudentProgramCrawler programCrawler1 = new StudentProgramCrawler(_sessionId, url1, programSubjectSaver, preParGetter);
            ReportProgress(30);
            StudentProgramCrawler programCrawler2 = new StudentProgramCrawler(_sessionId, url2, programSubjectSaver, preParGetter);
            ReportProgress(50);
            StudentProgramCrawler programCrawler3 = new StudentProgramCrawler(_sessionId, url3, programSubjectSaver, preParGetter);
            ReportProgress(70);
            StudentProgramCrawler programCrawler4 = new StudentProgramCrawler(_sessionId, url4, programSubjectSaver, preParGetter);
            ReportProgress(90);
            ProgramDiagram diagram = new ProgramDiagram(programCrawler1.Root, programCrawler2.Root,
                                                        programCrawler3.Root, programCrawler4.Root);
            return diagram;
        }

        private void ReportProgress(int percentProgress)
        {
            if (_backgroundWorker != null)
            {
                _backgroundWorker.ReportProgress(percentProgress);
            }
        }
    }
}
