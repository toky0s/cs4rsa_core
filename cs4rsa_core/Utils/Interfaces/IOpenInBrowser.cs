namespace Cs4rsa.Utils.Interfaces
{
    public interface IOpenInBrowser
    {
        /// <summary>
        /// Mở file HTML hoặc truy cập web trong trình duyệt.
        /// </summary>
        /// <param name="urlOrPath">Url hoặc đường dẫn tới file HTML.</param>
        void Open(string urlOrPath);
    }
}
