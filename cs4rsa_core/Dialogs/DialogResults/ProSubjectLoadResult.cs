using cs4rsa_core.Services.ProgramSubjectCrawlerSvc.DataTypes;

namespace cs4rsa_core.Dialogs.DialogResults
{
    public class ProSubjectLoadResult
    {
        private ProgramDiagram _programDiagram;
        public ProgramDiagram ProgramDiagram => _programDiagram;
        public ProSubjectLoadResult(ProgramDiagram programDiagram)
        {
            _programDiagram = programDiagram;
        }
    }
}
