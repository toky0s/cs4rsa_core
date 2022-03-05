using System.Collections.Generic;

namespace cs4rsa_core.Dialogs.DialogResults
{
    /// <summary>
    /// Là một item trong kết quả trả về của Trình Quản lý Phiên.
    /// </summary>
    public class SubjectInfoData
    {
        public string SubjectCode { get; set; }
        public string ClassGroup { get; set; }
        public string SubjectName { get; set; }
        public string RegisterCode { get; set; }
    }

    /// <summary>
    /// Là kết quả trả về của Trình Quản lý Phiên.
    /// </summary>
    public class SessionManagerResult
    {
        private IEnumerable<SubjectInfoData> _subjectInfoDatas;
        public IEnumerable<SubjectInfoData> SubjectInfoDatas
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
        public SessionManagerResult(IEnumerable<SubjectInfoData> subjectInfoDatas)
        {
            _subjectInfoDatas = subjectInfoDatas;
        }
    }
}
