using cs4rsa_core.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;

namespace cs4rsa_core.BaseClasses
{
    public abstract class ViewModelBase : ObservableRecipient
    {
        /// <summary>
        /// Mở Dialog
        /// </summary>
        /// <param name="uc">Dialog UC</param>
        protected void OpenDialog(IDialog uc)
        {
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).OpenModal(uc);
        }

        /// <summary>
        /// Đóng Dialog hiện tại đang hiển thị
        /// </summary>
        protected void CloseDialog()
        {
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).CloseModal();
        }
    }
}
