using cs4rsa_core.BaseClasses;
using System.Windows;

namespace cs4rsa_core.Dialogs.DialogServices
{
    /// <summary>
    /// Dialog ViewModel.
    /// <para>View binding với viewmodel này cần có thêm dialogExtension:DialogExtension.DialogResult="{Binding Result}"
    /// </para>
    /// </summary>
    /// <typeparam name="T">Dialog Result được trả về khi người dùng đóng Dialog</typeparam>
    public abstract class DialogViewModelBase<T>: ViewModelBase
    {
        private bool? _result;
        public bool? Result
        {
            get
            {
                return _result;
            }
            set
            {
                _result = value;
                OnPropertyChanged();
            }
        }

        public T UserDialogResult { get; set; }

        /// <summary>
        /// Đóng Dialog thông qua một CommandParameter truyền vào chính Dialog Window
        /// đó để thực hiện gán DialogResult.
        /// </summary>
        /// <param name="dialog">Một Window (dialog) được truyền thông qua XAML.</param>
        /// <param name="result">Kết quả trả về của Dialog</param>
        public void CloseDialogWithResult(Window dialog, T result)
        {
            UserDialogResult = result;
            if (dialog != null)
                dialog.DialogResult = true;
        }


        /// <summary>
        /// Đóng dialog thông qua một attacted property Result của dialog được binding
        /// với Result của view model này.
        /// </summary>
        /// <param name="result"></param>
        public void CloseDialogWithResult(T result)
        {
            UserDialogResult = result;
            Result = true;
        }
    }
}
