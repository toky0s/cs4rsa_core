using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace cs4rsa.Dialogs.MessageBoxService
{
    public interface IMessageBox
    {
        MessageBoxResult ShowMessage(string message);
        MessageBoxResult ShowMessage(string message, string caption);
        MessageBoxResult ShowMessage(string message, string caption, MessageBoxButton button);
        MessageBoxResult ShowMessage(string message, string caption, MessageBoxButton button, MessageBoxImage icon);
    }
}
