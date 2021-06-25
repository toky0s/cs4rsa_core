using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.Interfaces
{
    public interface IProgramSubjectSaver
    {
        void Save(string courseid, string subjectCode, List<string> prerequisites,
            List<string> parallels);
    }
}
