using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Cs4rsa.UI.Helper
{
    public class Debouncer
    {
        private DispatcherTimer _timer;
        private readonly int _interval;
        private readonly Action _action;

        public Debouncer(int intervalMs, Action action)
        {
            _interval = intervalMs;
            _action = action;
        }

        public void Debounce()
        {
            _timer?.Stop();
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(_interval)
            };
            _timer.Tick += (s, e) =>
            {
                _timer.Stop();
                _action.Invoke();
            };
            _timer.Start();
        }
    }
}
