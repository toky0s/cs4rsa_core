using Cs4rsa.App.Events.TopMenuEvents;

using Prism.Events;
using Prism.Mvvm;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsa.App.ViewModels
{
    internal class MainWindowViewModel: BindableBase
    {
        private int _screenIdx;
        public int ScreenIdx
        {
            get { return _screenIdx; }
            set { SetProperty(ref _screenIdx, value); }
        }

        public MainWindowViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<ScreenChangedEvent>().Subscribe((idx) => ScreenIdx = idx);
        }
    }
}
