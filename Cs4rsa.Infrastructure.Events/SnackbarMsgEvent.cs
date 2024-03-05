using Prism.Events;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsa.Infrastructure.Events
{
    /// <summary>
    /// Hiển thị một message trên Snackbar
    /// </summary>
    public class SnackbarMsgEvent: PubSubEvent<string>
    {
    }
}
