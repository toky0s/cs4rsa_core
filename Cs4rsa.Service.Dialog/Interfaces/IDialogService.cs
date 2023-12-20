using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Cs4rsa.Service.Dialog.Interfaces
{
    public interface IDialogService
    {
        void OpenDialog(UserControl userControl);
        void CloseDialog();
    }
}
