namespace ProgramSubjectCrawlerService.DataTypes.Enums
{
    /// <summary>
    /// Đại diện cho hình thức học của một Folder, có thể là bắt buộc
    /// có thể là Chọn n trong k môn có trong folder.
    /// Compulisory: Bắt buộc
    /// AllowSelection: Cho phép chọn n trong k môn
    /// </summary>
    public enum StudyMode
    {
        Compulsory,
        AllowSelection
    }
}
