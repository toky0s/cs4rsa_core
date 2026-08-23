using Cs4rsa.Service.Dialog.Events;
using Cs4rsa.Service.Dialog.Interfaces;

using Microsoft.Extensions.Logging;

using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Cs4rsa.Service.Dialog
{
    public class DialogService : IDialogService
    {
        private readonly IEventAggregator _eventAggregator;
        private Window _window;

        private readonly ILogger<IDialogService> _logger;

        public DialogService(
            IEventAggregator eventAggregator,
            ILogger<IDialogService> logger)
        {
            _eventAggregator = eventAggregator;
        }

        public void CloseDialog()
        {
            _eventAggregator.GetEvent<CloseDialogEvent>().Publish();
        }

        public void CloseDialog(object parameter)
        {
            
        }

        public void OpenDialog(UserControl userControl, bool isCloseOnClickAway = true)
        {
            _eventAggregator.GetEvent<OpenDialogEvent>().Publish(Tuple.Create(userControl, isCloseOnClickAway));
        }

        public void OpenDialog(UserControl userControl)
        {
            
            
                // Create a dialog that is wrapped in a Window to show the UserControl as a dialog
                Window dialogWindow = new Window
                {
                    Owner = _window,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Content = userControl,
                    SizeToContent = SizeToContent.WidthAndHeight,
                    ResizeMode = ResizeMode.NoResize,
                    WindowStyle = WindowStyle.ToolWindow,
                    ShowInTaskbar = false,
                    Title = (userControl.DataContext as DialogViewModelBase)?.DialogWindowName ?? "A Dialog"
                };

#if DEBUG
                // In debug mode, show the dialog as a non-modal window
                // to allow easier debugging of the dialog's UI and interactions
                dialogWindow.Show();
#else
                // In release mode, show the dialog as a modal window to 
                // block interaction with the main window
                dialogWindow.ShowDialog();           
#endif
                _logger.LogInformation($"Show dialog {dialogWindow.Title}");
            
        }

        public void RegisterWindow(Window w)
        {
            if (_window != null)
            {
                throw new InvalidOperationException("DialogService was already registered with a window.");
            }
            _window = w;
        }
    }
}
