using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;


using Cs4rsa.BaseClasses;
using Cs4rsa.Messages.Publishers;
using Cs4rsa.Models;

using MaterialDesignThemes.Wpf;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Cs4rsa.ViewModels.AutoScheduling
{
    internal partial class ResultViewModel : ViewModelBase
    {
        public ObservableCollection<CombinationModel> CombinationModels { get; set; }
        private List<CombinationModel> _combiModels;
        private int _currentIdx;

        [ObservableProperty]
        private int _batchSize;

        [ObservableProperty]
        private bool _isEnd;

        public RelayCommand GenCommand { get; set; }
        public RelayCommand DelCommand { get; set; }

        private readonly ISnackbarMessageQueue _snbMsgQueue;
        public ResultViewModel(ISnackbarMessageQueue snbMsgQueue)
        {
            _currentIdx = 0;
            _snbMsgQueue = snbMsgQueue;

            CombinationModels = new();
            BatchSize = 5;
            IsEnd = false;

            DelCommand = new(OnDel);
            GenCommand = new(
                OnGen,
                () => _combiModels != null
                    && _combiModels.Any()
                    && !_isEnd
            );

            Messenger.Register<AutoVmMsgs.AddCombinationsMsg>(this, (r, m) =>
            {
                _combiModels = m.Value;
                _isEnd = false;
                CombinationModels.Clear();
                GenCommand.NotifyCanExecuteChanged();
                DelCommand.NotifyCanExecuteChanged();
            });
        }

        private void OnGen()
        {
            int combiCount = _combiModels.Count;
            if (_batchSize > combiCount)
            {
                foreach (CombinationModel cm in _combiModels)
                {
                    CombinationModels.Insert(0, cm);
                }
                _isEnd = true;
            }
            else
            {
                int count = 0;
                for (
                    int i = _currentIdx;
                    (i < combiCount && count < _batchSize);
                    i++
                )
                {
                    CombinationModels.Insert(0, _combiModels[i]);
                    count++;
                }
                _currentIdx = CombinationModels.Count;
                _isEnd = _currentIdx == _combiModels.Count;
            }

            if (_isEnd)
            {
                _snbMsgQueue.Enqueue("Đã gen hết");
                GenCommand.NotifyCanExecuteChanged();
            }
        }

        private void OnDel()
        {
            CombinationModels.Clear();
            _currentIdx = 0;
            _isEnd = false;

            GenCommand.NotifyCanExecuteChanged();
            DelCommand.NotifyCanExecuteChanged();
        }
    }
}
