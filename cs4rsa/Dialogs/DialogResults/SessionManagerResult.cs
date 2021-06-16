using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.Dialogs.DialogResults
{
    /// <summary>
    /// Là một item trong kết quả trả về của trình quản lý phiên.
    /// </summary>
    public class SubjectInfoData
    {
        public string SubjectCode { get; set; }
        public string ClassGroup { get; set; }
        public string SubjectName { get; set; }
    }

    /// <summary>
    /// Là kết quả trả về của trình quản lý phiên.
    /// </summary>
    public class SessionManagerResult
    {
        private List<SubjectInfoData> _subjectInfoDatas;
        public List<SubjectInfoData> SubjectInfoDatas
        {
            get
            {
                return _subjectInfoDatas;
            }
            set
            {
                _subjectInfoDatas = value;
            }
        }
        public SessionManagerResult(List<SubjectInfoData> subjectInfoDatas)
        {
            _subjectInfoDatas = subjectInfoDatas;
        }

        public void Add(SubjectInfoData subjectInfo)
        {
            _subjectInfoDatas.Add(subjectInfo);
        }
    }
}
