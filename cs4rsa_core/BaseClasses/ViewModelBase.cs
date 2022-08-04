using cs4rsa_core.ViewModels;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Windows;

namespace cs4rsa_core.BaseClasses
{
    public class ViewModelBase : ObservableRecipient
    {
        /// <summary>
        /// Mở Dialog
        /// </summary>
        /// <param name="uc">Dialog UC</param>
        protected void OpenD(IDialog uc)
        {
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).OpenDialog(uc);
        }

        /// <summary>
        /// Đóng Dialog hiện tại đang hiển thị
        /// </summary>
        protected void CloseD()
        {
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).CloseDialog();
        }
    }
}
