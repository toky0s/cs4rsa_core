using CommunityToolkit.Mvvm.ComponentModel;

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
        public ViewModelBase() : base(((App)Application.Current).Messenger)
        {
        }

        /// <summary>
        /// Mở Dialog
        /// </summary>
        /// <param name="uc">Dialog UC</param>
        protected virtual void OpenDialog(IDialog uc)
        {
            ((MainWindowViewModel)Application.Current.MainWindow.DataContext).OpenModal(uc);
        }

        /// <summary>
        /// Đóng Dialog hiện tại đang hiển thị
        /// </summary>
        protected virtual void CloseDialog()
        {
            ((MainWindowViewModel)Application.Current.MainWindow.DataContext).CloseModal();
        }

        /// <summary>
        /// <inheritdoc cref="MainWindow.Goto(int)"/>
        /// </summary>
        /// <param name="scrIdx">Screen Index</param>
        protected virtual async Task GotoScreen(int scrIdx)
        {
            await ((MainWindow)Application.Current.MainWindow).Goto(scrIdx);
        }

        /// <summary>
        /// Set value for MainWindow's IsEnabled.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>prevent: true  -> MainWindow was enabled</item>
        /// <item>prevent: false -> MainWindow was disabled</item>
        /// </list>
        /// </remarks>
        /// <param name="prevent"></param>
        protected virtual void PreventOperation(bool prevent)
        {
            ((MainWindowViewModel)Application.Current.MainWindow.DataContext).IsWindowEnable = !prevent;
        }

        /// <summary>
        /// Truy cập các ViewModel có chứa trong container
        /// </summary>
        /// <typeparam name="T">ViewModel Type</typeparam>
        protected static T GetViewModel<T>()
        {
            object o = ((App)Application.Current).Container.GetService(typeof(T));
            if (o != null && o is T t)
            {
                return t;
            }
            else
            {
                throw new Exception(VmConstants.NotFoundViewModelException);
            }
        }

        /// <summary>
        /// Chặn đóng Dialog.
        /// </summary>
        /// <param name="isPrevent"></param>
        protected static void PreventCloseDialog(bool isPrevent)
        {
            ((MainWindowViewModel)Application.Current.MainWindow.DataContext).IsCloseOnClickAway = !isPrevent;
        }
    }
}
