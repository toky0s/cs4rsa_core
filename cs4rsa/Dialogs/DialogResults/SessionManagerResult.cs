using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.Dialogs.DialogResults
{
    public class SubjectInfoData
    {
        public string SubjectCode { get; set; }
        public string RegisterCode { get; set; }
    }

    public class SessionManagerResult
    {
        private List<SubjectInfoData> _subjects;
        public List<SubjectInfoData> Subjects
        {
            get
            {
                return _subjects;
            }
            set
            {
                _subjects = value;
            }
        }
        public SessionManagerResult(List<SubjectInfoData> subjects)
        {
            _subjects = subjects;
        }

        public void Add(SubjectInfoData subjectInfo)
        {
            _subjects.Add(subjectInfo);
        }
    }
}
