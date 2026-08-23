using Cs4rsa.Service.Notification.Models;
using System.Windows;
using System.Windows.Controls;

namespace Cs4rsa.Views.UserControls
{
    /// <summary>
    /// Interaction logic for NotificationItem
    /// </summary>
    public partial class NotificationItem : UserControl
    {


        public Notification Notification
        {
            get { return (Notification)GetValue(NotificationProperty); }
            set { SetValue(NotificationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Notification.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NotificationProperty =
            DependencyProperty.Register(nameof(Notification), typeof(Notification), typeof(NotificationItem), null);


        public NotificationItem()
        {
            InitializeComponent();
        }
    }
}
