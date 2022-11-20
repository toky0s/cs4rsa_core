namespace Cs4rsa.Services.ProgramSubjectCrawlerSvc.DataTypes.Enums
{
    /// <summary>
    /// Đại diện cho hình thức học của một Folder, có thể là bắt buộc
    /// có thể là Chọn n trong k môn có trong folder.
    /// </summary>
    public enum StudyMode
    {
        /**
         * Bắt buộc
         */
        Compulsory,

        /**
         * Phải hoàn thành n môn trong k môn.
         */
        AllowSelection
    }
}
