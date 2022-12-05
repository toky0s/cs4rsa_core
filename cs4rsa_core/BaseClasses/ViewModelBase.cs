using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.Constants;
using Cs4rsa.ViewModels;

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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
                throw new Exception(VMConstants.EX_NOT_FOUND_VIEWMODEL);
            }
        }

        /// <summary>
        /// Đồng bộ - OpenDialogAndDoSomethingThenClose
        /// 
        /// 1. Mở một hộp thoại
        /// 2. Thực hiện một hành động
        /// 3. Đóng hộp thoại khi thực hiện xong.
        /// </summary>
        /// <param name="thisDialog">IDialog</param>
        /// <param name="doSomething">Hành động cần thực hiện.</param>
        protected void OpenDialogAndDoSomething(IDialog thisDialog, Action doSomething)
        {
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).OpenModal(thisDialog);
            doSomething();
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).CloseModal();
        }

        /// <summary>
        /// Bất đồng bộ - OpenDialogAndDoSomethingThenClose
        /// <para>
        /// Sử dụng khi thời điểm đóng Dialog luôn được xác định trước
        /// là lúc <paramref name="doSomething"/> thực hiện xong.
        /// </para>
        /// <list type="number">
        /// <item>
        /// Mở một hộp thoại, truyền instance của thisDialog (<typeparamref name="T"/>) cho doSomething.
        /// </item>
        /// <item>
        /// Thực hiện một hành động.
        /// </item>
        /// <item>
        /// Đóng hộp thoại khi thực hiện xong.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">
        ///     Kế thừa từ UserControl.
        ///     Triển khai từ IDialog.
        /// </typeparam>
        /// <param name="thisDialog">Dialog cần mở</param>
        /// <param name="doSomething">Hành động cần thực hiện.</param>
        /// <returns>Task</returns>
        protected async Task OpenDialogAndDoSomethingThenClose<T>(T thisDialog, Func<T, Task> doSomething) where T : UserControl, IDialog
        {
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).OpenModal(thisDialog);
            await doSomething(thisDialog);
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).CloseModal();
        }
    }
}
