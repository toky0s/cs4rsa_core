using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace cs4rsa_core.Dialogs.DialogServices
{
    /// <summary>
    /// Khởi chạy DialogService.
    /// </summary>
    /// <typeparam name="T">Dialog result được trả về khi người dùng đóng Dialog</typeparam>
    public static class DialogService<T>
    {
        /// <summary>
        /// Hiển thị Dialog.
        /// </summary>
        /// <param name="vm">Đây là ViewModel của Dialog.</param>
        /// <param name="dialogWindow">Dialog Window</param>
        /// <param name="owner">Cửa sổ gọi Dialog Window</param>
        /// <returns>T</returns>
        public static T OpenDialog(DialogViewModelBase<T> vm, Window dialogWindow, Window owner)
        {
            if (owner != null)
                dialogWindow.Owner = owner;
            dialogWindow.DataContext = vm;
            dialogWindow.ShowDialog();
            T result = (dialogWindow.DataContext as DialogViewModelBase<T>).UserDialogResult;
            return result;
        }
    }
}
