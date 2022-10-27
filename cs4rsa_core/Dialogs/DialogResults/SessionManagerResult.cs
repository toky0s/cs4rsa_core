using System.Collections.Generic;

namespace cs4rsa_core.Dialogs.DialogResults
{
    /// <summary>
    /// Là một item trong kết quả trả về của Trình Quản lý Phiên.
    /// </summary>
    public record SubjectInfoData
    {
        public string SubjectCode { get; init; }
        public string ClassGroup { get; init; }
        public string SubjectName { get; init; }
        public string RegisterCode { get; init; }
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
