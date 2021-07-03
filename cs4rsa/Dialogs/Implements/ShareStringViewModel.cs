using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using cs4rsa.Dialogs.DialogResults;
using cs4rsa.Dialogs.DialogService;

namespace cs4rsa.Dialogs.Implements
{
    public class ShareStringViewModel: DialogViewModelBase<ShareStringResult>
    {
        private string _shareString;
        public string ShareString
        {
            get
            {
                return _shareString;
            }
            set
            {
                _shareString = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand CopyCommand { get; set; }

        public ShareStringViewModel(string shareString)
        {
            ShareString = shareString;
            CopyCommand = new RelayCommand(OnCopy, () => true);
        }

        private void OnCopy(object obj)
        {
            Clipboard.SetData(DataFormats.Text, _shareString);
        }
    }
}
