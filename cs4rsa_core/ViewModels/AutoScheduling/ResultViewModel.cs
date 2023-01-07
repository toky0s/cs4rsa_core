using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;


using Cs4rsa.BaseClasses;
using Cs4rsa.Dialogs.DialogViews;
using Cs4rsa.Messages.Publishers;
using Cs4rsa.Models;

using System.Collections.ObjectModel;
using System.Windows;

namespace Cs4rsa.ViewModels.AutoScheduling
{
    internal partial class ResultViewModel : ViewModelBase
    {
        private AutoFilterUC _autoFilterUc;
        public ObservableCollection<CombinationModel> CombinationModels { get; set; }

        [ObservableProperty]
        private CombinationModel _sltCombiModel;

        public RelayCommand OpenFilterCommand { get; set; }

        public ResultViewModel()
        {
            _autoFilterUc = new();

            CombinationModels = new();

            OpenFilterCommand = new(OnOpenFilter);


            Messenger.Register<AutoVmMsgs.AddCombinationMsg>(this, (r, m) =>
            {
                Application.Current.Dispatcher.InvokeAsync(() => CombinationModels.Add(m.Value));
            });
        }


        private void OnOpenFilter()
        {
            OpenDialog(_autoFilterUc);
        }
    }
}
