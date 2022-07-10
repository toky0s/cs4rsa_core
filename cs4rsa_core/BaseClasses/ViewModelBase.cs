using cs4rsa_core.ViewModels;

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;

namespace cs4rsa_core.BaseClasses
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        protected static string RootFolderPath = AppDomain.CurrentDomain.BaseDirectory;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        /// <summary>
        /// Mở Dialog
        /// </summary>
        /// <param name="uc">Dialog UC</param>
        #pragma warning disable CA1822 // Mark members as static
        protected void OpenD(IDialog uc)
        #pragma warning restore CA1822 // Mark members as static
        {
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).OpenDialog(uc);
        }

        /// <summary>
        /// Đóng Dialog hiện tại đang hiển thị
        /// </summary>
        #pragma warning disable CA1822 // Mark members as static
        protected void CloseD()
        #pragma warning restore CA1822 // Mark members as static
        {
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).CloseDialog();
        }

        /// <summary>
        /// Mở Folder
        /// 
        /// Lưu ý tham số path kết thúc bằng \\ để đánh dấu là một Folder.
        /// Xem thêm: https://stackoverflow.com/questions/1132422/open-a-folder-using-process-start
        /// </summary>
        /// <param name="path">Đường dẫn tới folder</param>
        protected static void OpenFolderWithExplorer(string path)
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = path,
                UseShellExecute = true,
                Verb = "open"
            });
        }
    }
}
