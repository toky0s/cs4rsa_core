using cs4rsa.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace cs4rsa.Dialogs.DialogService
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">Dialog Result được trả về khi người dùng đóng Dialog</typeparam>
    public abstract class DialogViewModelBase<T>: NotifyPropertyChangedBase
    {
        public T UserDialogResult { get; set; }

        /// <summary>
        /// Đóng Dialog thông qua một CommandParameter truyền vào chính Dialog
        /// đó để thực hiện gán DialogResult.
        /// </summary>
        /// <param name="dialog">Một Window (dialog) được truyền thông qua XAML.</param>
        /// <param name="result">Kết quả trả về của Dialog</param>
        public void CloseDialogWithResult(Window dialog,T result)
        {
            UserDialogResult = result;
            if (dialog != null)
                dialog.DialogResult = true;
        }
    }
}
