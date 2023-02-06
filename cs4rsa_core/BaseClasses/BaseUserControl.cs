using CommunityToolkit.Mvvm.Messaging;
using System.Windows;
using System.Windows.Controls;

namespace Cs4rsa.BaseClasses
{
    public class BaseUserControl: UserControl
    {
        public IMessenger Messenger { get; set; }
        public BaseUserControl() : base()
        {
            Loaded += BaseUserControl_Loaded;
        }

        private void BaseUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Messenger = ((App)Application.Current).Messenger;
        }
    }
}
