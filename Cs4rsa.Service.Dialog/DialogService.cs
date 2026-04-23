using Cs4rsa.Service.Dialog.Events;
using Cs4rsa.Service.Dialog.Interfaces;

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
        public DialogService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void CloseDialog()
        {
            _eventAggregator.GetEvent<CloseDialogEvent>().Publish();
        }

        public void OpenDialog(UserControl userControl, bool isCloseOnClickAway = true)
        {
            _eventAggregator.GetEvent<OpenDialogEvent>().Publish(Tuple.Create(userControl, isCloseOnClickAway));
        }

        public void OpenDialog<View, ViewModel>(View userControl, ViewModel viewModel)
            where View : UserControl
            where ViewModel : class
        {
            
            _eventAggregator.GetEvent<OpenDialogEvent_v2>().Publish(userControl);
        }
    }
}
