using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.BasicData
{
    /// <summary>
    /// Đại diện cho các môn tiên quyết và môn song hành của một môn nào đó.
    /// </summary>
    public class PreParSubject
    {
        private readonly List<string> _preSubjects;
        private readonly List<string> _parSubjects;
        public List<string> PreSubjects => _preSubjects;
        public List<string> ParSubjects => _parSubjects;
        public PreParSubject(List<string> preSubjects, List<string> parSubjects)
        {
            _preSubjects = preSubjects;
            _parSubjects = parSubjects;
        }
    }
}
