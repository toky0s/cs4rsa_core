using cs4rsa.BasicData;
using cs4rsa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.Dialogs.DialogResults
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
