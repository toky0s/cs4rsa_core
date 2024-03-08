using Prism.Mvvm;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsa.Module.Shared
{
    /// <summary>
    /// Các dialog cần show một dialog khác bên trong phải kế thừa từ lớp này.
    /// Ví dụ: hiển thị dialog xác nhận Yes/No, ...
    /// </summary>
    public abstract class ShowDialogViewModelAbstract: BindableBase
    {
        private object _dialog;

        /// <summary>
        /// Dialog được định nghĩa dưới dạng một User Control.
        /// </summary>
        public object Dialog
        {
            get { return _dialog; }
            set { SetProperty(ref _dialog, value); }
        }

        private bool _isCloseOnClickAway;

        /// <summary>
        /// Nếu bằng True, người dùng có thể click ra bên ngoài dialog để đóng dialog.
        /// Ngược lại họ không thể.
        /// </summary>
        public bool IsCloseOnClickAway
        {
            get { return _isCloseOnClickAway; }
            set { SetProperty(ref _isCloseOnClickAway, value); }
        }

        private bool _isOpen;
        /// <summary>
        /// Set thành True để hiển thị dialog. Ngược lại set thành False để đóng dialog.
        /// </summary>
        public bool IsOpen
        {
            get { return _isOpen; }
            set { SetProperty(ref _isOpen, value); }
        }
    }
}
