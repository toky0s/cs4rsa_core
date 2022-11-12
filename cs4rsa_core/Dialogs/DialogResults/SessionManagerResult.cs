using System.Collections.Generic;

namespace cs4rsa_core.Dialogs.DialogResults
{
    /// <summary>
    /// Là một item trong kết quả trả về của Trình Quản lý Phiên.
    /// </summary>
    public record SubjectInfoData
    {
        public readonly string SubjectCode;
        public readonly string ClassGroup;
        public readonly string SubjectName;
        public readonly string RegisterCode;
        public readonly string SchoolClassName;

        public SubjectInfoData(
            string subjectCode, 
            string classGroup,
            string subjectName,
            string registerCode,
            string schoolClassName
        )
        {
            SubjectCode = subjectCode;
            ClassGroup = classGroup;
            SubjectName = subjectName;
            RegisterCode = registerCode;
            SchoolClassName = schoolClassName;
        }
    }

    /// <summary>
    /// Là kết quả trả về của Trình Quản lý Phiên.
    /// </summary>
    public record SessionManagerResult
    {
        public readonly IEnumerable<SubjectInfoData> SubjectInfoDatas;
        public SessionManagerResult(IEnumerable<SubjectInfoData> subjectInfoDatas)
        {
            SubjectInfoDatas = subjectInfoDatas;
        }
    }
}
