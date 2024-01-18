using Cs4rsa.Service.Dialog.Events;
using Cs4rsa.Service.Dialog.Interfaces;

using Prism.Events;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
