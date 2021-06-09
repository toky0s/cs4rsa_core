using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.Dialogs.DialogService;
using cs4rsa.Dialogs.Implements;

namespace cs4rsa.ViewModels
{
    class MainWindowViewModel
    {
        public RelayCommand SessionManagerCommand;

        public MainWindowViewModel()
        {
            SessionManagerCommand = new RelayCommand(OnOpenSessionManager, () => true);
        }

        private void OnOpenSessionManager(object obj)
        {
            
        }
    }
}
