using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.Constants;
using Cs4rsa.ViewModels;
using Cs4rsa.Views;

using System;
using System.Threading.Tasks;
using System.Windows;

namespace Cs4rsa.BaseClasses
{
    public abstract class ViewModelBase : ObservableRecipient
    {
        public ViewModelBase() : base(WeakReferenceMessenger.Default)
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
        /// <inheritdoc cref="MainWindow.Goto(int)"/>
        /// </summary>
        /// <param name="scrIdx">Screen Index</param>
        protected virtual async Task GotoScreen(int scrIdx)
        {
            await (Application.Current.MainWindow as MainWindow).Goto(scrIdx);
        }

        /// <summary>
        /// Set value for MainWindow's IsEnabled.
        /// 
        /// isEnabled: true  -> MainWindow was enabled
        /// isEnabled: false -> MainWindow was disabled
        /// </summary>
        /// <param name="isEnabled"></param>
        protected virtual void PreventOperation(bool prevent)
        {
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).IsWindowEnable = !prevent;
        }

        /// <summary>
        /// Truy cập các ViewModel có chứa trong container
        /// </summary>
        /// <typeparam name="T">ViewModel Type</typeparam>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
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
        /// Chặn đóng Dialog.
        /// </summary>
        /// <param name="isPrevent"></param>
        protected static void PreventCloseDialog(bool isPrevent)
        {
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).IsCloseOnClickAway = !isPrevent;
        }
    }
}
