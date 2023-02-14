using CommunityToolkit.Mvvm.Messaging;

using System;
using System.Windows;
using System.Windows.Controls;

namespace Cs4rsa.BaseClasses
{
    public class BaseUserControl : UserControl
    {
        public IMessenger Messenger { get; set; }
        public IServiceProvider Container { get; set; }
        public BaseUserControl() : base()
        {
            Container = ((App)Application.Current).Container;
            Loaded += BaseUserControl_Loaded;
        }

        private void BaseUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Messenger = ((App)Application.Current).Messenger;
        }
    }
}
