using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsa.Service.Dialog
{
    public abstract class DialogViewModelBase: BindableBase
    {
        private string _dialogWindowName = "A Dialog";
        public string DialogWindowName
        {
            get { return _dialogWindowName; }
            set { SetProperty(ref _dialogWindowName, value); }
        }

        protected DialogViewModelBase(string dialogWindowName)
        {
            DialogWindowName = dialogWindowName;
        }
    }
}
