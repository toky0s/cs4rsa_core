using cs4rsa_core.ViewModels;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace cs4rsa_core.BaseClasses
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string property = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        /// <summary>
        /// Mở Dialog
        /// </summary>
        /// <param name="uc">Dialog UC</param>
        protected void OpenDialog(UserControl uc)
        {
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).OpenDialog(uc);
        }

        /// <summary>
        /// Đóng Dialog
        /// </summary>
        protected void CloseDialog()
        {
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).CloseDialog();
        }
    }
}
