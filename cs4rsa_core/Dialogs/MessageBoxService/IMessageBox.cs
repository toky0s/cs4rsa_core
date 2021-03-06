using System.Windows;

namespace cs4rsa_core.Dialogs.MessageBoxService
{
    public interface IMessageBox
    {
        MessageBoxResult ShowMessage(string message);
        MessageBoxResult ShowMessage(string message, string caption);
        MessageBoxResult ShowMessage(string message, string caption, MessageBoxButton button);
        MessageBoxResult ShowMessage(string message, string caption, MessageBoxButton button, MessageBoxImage icon);
    }
}
