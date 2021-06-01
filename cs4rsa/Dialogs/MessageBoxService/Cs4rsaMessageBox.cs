using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;



namespace cs4rsa.Dialogs.MessageBoxService
{
    class Cs4rsaMessageBox : IMessageBox
    {
        public MessageBoxResult ShowMessage(string message)
        {
            return MessageBox.Show(message);
        }

        public MessageBoxResult ShowMessage(string message, string caption)
        {
            throw new NotImplementedException();
        }

        public MessageBoxResult ShowMessage(string message, string caption, MessageBoxButton button)
        {
            throw new NotImplementedException();
        }

        public MessageBoxResult ShowMessage(string message, string caption, MessageBoxButton button, MessageBoxImage icon)
        {
            throw new NotImplementedException();
        }
    }
}
