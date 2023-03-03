using CommunityToolkit.Mvvm.Messaging;

using System;
using System.Windows;
using System.Windows.Controls;

namespace Cs4rsa.BaseClasses
{
    public class BaseUserControl : UserControl
    {
        protected IMessenger Messenger { get; private set; }
        protected IServiceProvider Container { get; }

        protected BaseUserControl()
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
