using Cs4rsa.Module.Shared;
using Cs4rsa.Service.Dialog;
using Cs4rsa.Service.Dialog.Events;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Cs4rsa.App.Views
{
    public abstract class MainWindowBase : Window
    {


        private readonly IEventAggregator _eventAggregator;
        protected MainWindowBase(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<OpenDialogEvent_v2>().Subscribe((view) =>
            {
                // Create a dialog that is wrapped in a Window to show the UserControl as a dialog
                Window dialogWindow = new Window
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Content = view,
                    SizeToContent = SizeToContent.WidthAndHeight,
                    ResizeMode = ResizeMode.NoResize,
                    WindowStyle = WindowStyle.ToolWindow,
                    ShowInTaskbar = false,
                    Title = (view.DataContext as DialogViewModelBase)?.DialogWindowName ?? "A Dialog"
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
            });
        }
    }
}
