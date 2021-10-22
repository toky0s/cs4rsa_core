using cs4rsa_core.BaseClasses;
using cs4rsa_core.Dialogs.DialogResults;
using Cs4rsaDatabaseService.DataProviders;
using CurriculumCrawlerService.Crawlers;
using ProgramSubjectCrawlerService.Crawlers;
using ProgramSubjectCrawlerService.DataTypes;
using SubjectCrawlService1.Crawlers.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa_core.Dialogs.Implements
{
    public class ProSubjectLoadViewModel: ViewModelBase
    {
        public string SpecialString { get; set; }

        public Action<ProSubjectLoadResult> CloseDialogCallback { get; set; }

        private readonly CurriculumCrawler _curriculumCrawler;
        private readonly Cs4rsaDbContext _cs4rsaDbContext;
        private readonly IPreParSubjectCrawler _preParSubjectCrawler;
        public ProSubjectLoadViewModel(CurriculumCrawler curriculumCrawler, Cs4rsaDbContext cs4rsaDbContext, IPreParSubjectCrawler preParSubjectCrawler)
        {
            _curriculumCrawler = curriculumCrawler;
            _cs4rsaDbContext = cs4rsaDbContext;
            _preParSubjectCrawler = preParSubjectCrawler;
        }


        /// <summary>
        /// Tải danh sách các môn thuộc chương trình học của sinh viên.
        /// </summary>
        public async Task Load()
        {
            ProgramDiagramCrawler programDiagramCrawler = new ProgramDiagramCrawler("", SpecialString, _curriculumCrawler, _cs4rsaDbContext, _preParSubjectCrawler);
            ProgramDiagram diagram = await programDiagramCrawler.ToProgramDiagram();

            ProSubjectLoadResult result = new(diagram);
            CloseDialogCallback.Invoke(result);
        }
    }
}
