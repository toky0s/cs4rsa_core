using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace cs4rsa.Dialogs
{
    public static class DialogService
    {
        public static DialogResult OpenDialog(DialogViewModelBase vm, Window owner)
        {
            DialogWindow dialogWindow = new DialogWindow();
            if (owner != null)
                dialogWindow.Owner = owner;
            dialogWindow.DataContext = vm;
            dialogWindow.ShowDialog();
            DialogResult result = (dialogWindow.DataContext as DialogViewModelBase).UserDialogResult;
            return result;
        }
    }
}
