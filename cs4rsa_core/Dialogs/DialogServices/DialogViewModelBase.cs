using cs4rsa_core.BaseClasses;

namespace cs4rsa_core.Dialogs.DialogServices
{
    /// <summary>
    /// Dialog ViewModel.
    /// <para>View binding với viewmodel này cần có thêm dialogExtension:DialogExtension.DialogResult="{Binding Result}"
    /// </para>
    /// </summary>
    /// <typeparam name="T">Dialog Result được trả về khi người dùng đóng Dialog</typeparam>
    public abstract class DialogViewModelBase<T> : ViewModelBase
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
    }
}
