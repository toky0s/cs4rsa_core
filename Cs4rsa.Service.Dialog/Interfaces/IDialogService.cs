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
        /// <summary>
        /// Mở một Dialog
        /// </summary>
        /// <param name="userControl">Dialog</param>
        /// <param name="isCloseOnClickAway">Nếu True, cho phép đóng Dialog khi click ra ngoài. Ngược lại, không cho phép.</param>
        void OpenDialog(UserControl userControl, bool isCloseOnClickAway = true);
        void CloseDialog();
    }
}
