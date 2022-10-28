using cs4rsa_core.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using cs4rsa_core.Constants;

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

        /// <summary>
        /// Truy cập hoặc tạo mới các ViewModel có chứa trong container
        /// </summary>
        /// <typeparam name="T">ViewModel Type</typeparam>
        protected T GetViewModel<T>()
        {
            object o = ((App)Application.Current).Container.GetService(typeof(T));
            if (o != null && o is T t)
            {
                return t;
            }
            else
            {
                throw new System.Exception(VMConstants.EX_NOT_FOUND_VIEWMODEL);
            }
        }
    }
}
