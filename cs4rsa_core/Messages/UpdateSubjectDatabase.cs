using cs4rsa_core.BaseClasses;

namespace cs4rsa_core.Messages
{
    /// <summary>
    /// Yêu cầu update cơ sở dữ liệu môn học tới MainSchedulingViewModel.
    /// </summary>
    public class UpdateSubjectDatabase : Cs4rsaMessage
    {
        public UpdateSubjectDatabase(object source) : base(source)
        {
        }
    }
}
