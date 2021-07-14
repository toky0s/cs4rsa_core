using cs4rsa.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.Messages
{
    /// <summary>
    /// Message này gửi một thông báo tới MainViewModel
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
