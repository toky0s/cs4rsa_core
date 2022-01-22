using cs4rsa_core.BaseClasses;

namespace cs4rsa_core.Messages
{
    /// <summary>
    /// Message này gửi một thông báo tới MainWindowViewModel
    /// hiển thị một Snackbar Message lên màn hình. Giá trị
    /// của source là thông báo mà bạn cần hiển thị.
    /// </summary>
    public class Cs4rsaSnackbarMessage : Cs4rsaMessage
    {
        public new string Source;
        public Cs4rsaSnackbarMessage(string source) : base(source)
        {
            Source = source;
        }
    }
}
