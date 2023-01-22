using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.Constants;
using Cs4rsa.ViewModels;

using System;
using System.Windows;

namespace Cs4rsa.BaseClasses
{
    public abstract class ViewModelBase : ObservableRecipient
    {
        public ViewModelBase() : base(StrongReferenceMessenger.Default)
        {

        }

        /// <summary>
        /// Mở Dialog
        /// </summary>
        /// <param name="uc">Dialog UC</param>
        protected virtual void OpenDialog(IDialog uc)
        {
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).OpenModal(uc);
        }

        /// <summary>
        /// Đóng Dialog hiện tại đang hiển thị
        /// </summary>
        protected virtual void CloseDialog()
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
                throw new Exception(VMConstants.EX_NOT_FOUND_VIEWMODEL);
            }
        }
    }
}
