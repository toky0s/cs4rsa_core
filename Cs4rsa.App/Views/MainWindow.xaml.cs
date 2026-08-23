using Cs4rsa.App.ViewModels;
using Cs4rsa.Module.ManuallySchedule.Views;
using Cs4rsa.Module.Shared;

using DryIoc;

using Microsoft.Extensions.Logging;

using Prism.Events;
using Prism.Regions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cs4rsa.App.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private StatusWindow _statusWindow;
        public MainWindow(IRegionManager regionManager)
        {
            InitializeComponent();
        }

        private void Button_ShowNetworkStatus_Click(object sender, RoutedEventArgs e)
        {
            // Tạo và hiển thị cửa sổ
            if (_statusWindow != null && _statusWindow.IsVisible)
            {
                Mouse.Capture(null);
                Mouse.RemovePreviewMouseDownOutsideCapturedElementHandler(_statusWindow, OnClickOutsideWindow);
                _statusWindow.Close();
                _statusWindow = null;
            }
            else
            {
                var mainWindowDataContext = (MainWindowViewModel)DataContext;
                _statusWindow = new StatusWindow();
                ((StatusWindowViewModel)_statusWindow.DataContext).Connected = mainWindowDataContext.IsConnected;
                _statusWindow.Show();
                ReallocateStatusWindow();

                Mouse.Capture(_statusWindow);
                Mouse.AddPreviewMouseDownOutsideCapturedElementHandler(_statusWindow, OnClickOutsideWindow);
            }
        }

        private void ReallocateStatusWindow()
        {
            Point buttonPosition = ShowNetworkStatusButton.PointToScreen(new Point(0, 0));
            _statusWindow.Left = buttonPosition.X - _statusWindow.ActualWidth + ShowNetworkStatusButton.ActualWidth;
            _statusWindow.Top = buttonPosition.Y - _statusWindow.ActualHeight;
        }

        private void OnClickOutsideWindow(object sender, MouseButtonEventArgs e)
        {
            var statusWindow = (StatusWindow)sender;

            // Kiểm tra nếu click nằm trong tooltip → không đóng
            var hitResult = VisualTreeHelper.HitTest(statusWindow, e.GetPosition(statusWindow));
            if (hitResult != null)
            {
                return;
            }

            Mouse.Capture(null);
            Mouse.RemovePreviewMouseDownOutsideCapturedElementHandler(_statusWindow, OnClickOutsideWindow);
            statusWindow.Close();
        }
    }
}
