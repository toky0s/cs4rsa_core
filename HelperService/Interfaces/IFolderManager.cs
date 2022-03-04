namespace HelperService.Interfaces
{
    /// <summary>
    /// Class triển khai Interface này sẽ triển khai
    /// các phương thức thao tác với thư mục.
    /// </summary>
    public interface IFolderManager
    {
        string CreateFolderIfNotExists(string folderName);
    }
}
