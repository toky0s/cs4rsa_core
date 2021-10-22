using System.Windows;

namespace cs4rsa_core.Dialogs.MessageBoxService
{
    public class Cs4rsaMessageBox : IMessageBox
    {
        public MessageBoxResult ShowMessage(string message)
        {
            return MessageBox.Show(message);
        }

        public MessageBoxResult ShowMessage(string message, string caption)
        {
            return MessageBox.Show(message, caption);
        }

        public MessageBoxResult ShowMessage(string message, string caption, MessageBoxButton button)
        {
            return MessageBox.Show(message, caption, button);
        }

        public MessageBoxResult ShowMessage(string message, string caption, MessageBoxButton button, MessageBoxImage icon)
        {
            return MessageBox.Show(message, caption, button, icon);
        }
    }
}
