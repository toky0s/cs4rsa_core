using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cs4rsa.Infrastructure.Common
{
    public class JSFunctionality
    {
        public static CancellationTokenSource SetTimeout(Action action, int millis)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            _ = Task.Run(() => {
                Thread.Sleep(millis);
                if (!cancellationToken.IsCancellationRequested)
                    action();
            }, cancellationToken);

            return cancellationTokenSource;
        }

        public static void ClearTimeout(CancellationTokenSource cts)
        {
            using (cts)
            {
                cts.Cancel();
            }
        }
    }
}
