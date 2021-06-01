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

        public void CloseDialogWithResult(Window dialog,T result)
        {
            UserDialogResult = result;
            if (dialog != null)
                dialog.DialogResult = true;
        }
    }
}
