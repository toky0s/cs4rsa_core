using cs4rsa.BasicData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.Models
{
    public class ProgramSubjectModel
    {
        private ProgramSubject _programSubject;
        public string SubjectCode => _programSubject.SubjectCode;
        public string SubjectName => _programSubject.SubjectName;
        public ProgramSubjectModel(ProgramSubject programSubject)
        {
            _programSubject = programSubject;
        }
    }
}
