using Prism.Events;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Cs4rsa.Service.Dialog.Events
{
    /// <summary>
    /// Payload: UserControl là dialog cần bật, 
    /// bool là True thì cho phép đóng dialog khi click ra ngoài, ngược lại không cho phép.
    /// </summary>
    public class OpenDialogEvent: PubSubEvent<Tuple<UserControl, bool>>
    {
    }
}
