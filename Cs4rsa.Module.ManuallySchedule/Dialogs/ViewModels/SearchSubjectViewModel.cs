using Cs4rsa.Database.Interfaces;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Xeplich.Service.Search;

namespace Cs4rsa.Module.ManuallySchedule.Dialogs.ViewModels
{
    public class SearchResult
    {
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public string Discipline { get; set; }
        public string Keyword { get; set; }
        public string SubjectDescription { get; set; }

        // Color sẽ được fill sau khi có kết quả tìm kiếm
        public string Color { get; set; }
        // Cờ này True nếu kết quả này đã được tải xuống trong màn hình chính
        public bool IsDownloaded { get; set; }
        public string DisplayedText { get; set; }
    }

    public class SearchSubjectViewModel : BindableBase, IDialogAware
    {
        private int _hitCount;
        public int HitCount
        {
            get { return _hitCount; }
            set { SetProperty(ref _hitCount, value); }
        }
        private int _totalHits;
        public int TotalHits
        {
            get { return _totalHits; }
            set { SetProperty(ref _totalHits, value); }
        }
        private HashSet<string> _downloadSubjectCodes = new HashSet<string>();
        private ObservableCollection<SearchResult> _subjectResults = new ObservableCollection<SearchResult>();
        public ObservableCollection<SearchResult> SubjectResults
        {
            get { return _subjectResults; }
            set { SetProperty(ref _subjectResults, value); }
        }
        public string Title => "Search";

        public event Action<IDialogResult> RequestClose;

        private DelegateCommand _keyUpCommand;
        public DelegateCommand KeyUpCommand =>
            _keyUpCommand ?? (_keyUpCommand = new DelegateCommand(ExecuteKeyUpCommand, CanExecuteKeyUpCommand));

        void ExecuteKeyUpCommand()
        {
            InternalMoveSelectedResult(false);
        }

        bool CanExecuteKeyUpCommand()
        {
            return true;
        }

        private DelegateCommand _returnCommand;
        public DelegateCommand ReturnCommand =>
            _returnCommand ?? (_returnCommand = new DelegateCommand(ExecuteReturnCommand, CanExecuteReturnCommand));

        void ExecuteReturnCommand()
        {
            if (_selectedSearchResult != null)
            {
                var parameter = new DialogParameters
                {
                    { "SelectedSearchResult", _selectedSearchResult }
                };
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK, parameter));
            }
        }

        bool CanExecuteReturnCommand()
        {
            // Enter command sẽ execute được khi subject
            // được chọn chưa được tải ở ngoài màn hình chính.
            var subjectCode = $"{_selectedSearchResult.Discipline} {_selectedSearchResult.Keyword}";
            return !_downloadSubjectCodes.Contains(subjectCode);
        }

        private DelegateCommand _keyDownCommand;
        public DelegateCommand KeyDownCommand =>
            _keyDownCommand ?? (_keyDownCommand = new DelegateCommand(ExecuteKeyDownCommand, CanExecuteKeyDownCommand));

        void ExecuteKeyDownCommand()
        {
            InternalMoveSelectedResult(true);
        }

        void InternalMoveSelectedResult(bool isNext)
        {
            if (SubjectResults.Count == 0)
                return;

            int currIndex = _selectedSearchResult == null
                ? (isNext ? -1 : SubjectResults.Count)
                : SubjectResults.IndexOf(_selectedSearchResult);

            int nextIndex;

            if (isNext)
            {
                nextIndex = (currIndex + 1) % SubjectResults.Count;
            }
            else
            {
                nextIndex = (currIndex - 1 + SubjectResults.Count) % SubjectResults.Count;
            }

            SelectedSearchResult = SubjectResults[nextIndex];
            // Kiểm tra thực thi của Enter mỗi lần thay đổi Selected Subject Result
            ReturnCommand.RaiseCanExecuteChanged();
        }


        bool CanExecuteKeyDownCommand()
        {
            return true;
        }

        private SearchResult _selectedSearchResult;
        public SearchResult SelectedSearchResult
        {
            get { return _selectedSearchResult; }
            set { SetProperty(ref _selectedSearchResult, value); }
        }

        private DelegateCommand _closePopupCommand;
        public DelegateCommand ClosePopupCommand =>
            _closePopupCommand ?? (_closePopupCommand = new DelegateCommand(ExecuteClosePopupCommand, CanExecuteClosePopupCommand));

        private DelegateCommand<string> _searchCommand;
        public DelegateCommand<string> SearchCommand =>
            _searchCommand ?? (_searchCommand = new DelegateCommand<string>(ExecuteSearchCommand, CanExecuteSearchCommand));

        void ExecuteSearchCommand(string keyword)
        {
            InternalSearch(keyword);
        }

        private void InternalSearch(string keyword)
        {
            SubjectResults.Clear();

            _indexBuilder.SearchWithBoost(out List<DataModel> result, out int totalHits, keyword);
            TotalHits = totalHits;
            HitCount = result.Count;
            
            var tempSubjectResults = result.Select(item =>
            {
                return new SearchResult
                {
                    SubjectCode = item.SubjectCode,
                    SubjectName = item.SubjectName,
                    Discipline = item.Discipline,
                    Keyword = item.Keyword,
                    SubjectDescription = item.SubjectDescription,
                    DisplayedText = item.DisplayedText,
                    // Nếu đã có sẵn trong danh sách đã tải rồi thì chuyển cờ về True
                    IsDownloaded = _downloadSubjectCodes.Contains($"{item.Discipline} {item.Keyword}")
                };
            });
            SubjectResults.AddRange(tempSubjectResults);

            // TODO: Chuyển sang Early Loading để tránh N+1 Query.
            // Hiện tại đang load toàn bộ màu sắc của tất cả các subject code trong kết quả tìm kiếm.
            var db = _unitOfWork.Keywords.GetKeywordsBySubjectCode(result.Select(r => r.SubjectCode).ToArray());
            db.ForEach(k =>
            {
                var item = SubjectResults.FirstOrDefault(r => r.SubjectCode == k.Item2);
                if (item != null)
                {
                    item.Color = k.Item1;
                }
            });
        }

        bool CanExecuteSearchCommand(string keyword)
        {
            return true;
        }

        void ExecuteClosePopupCommand()
        {
            RequestClose?.Invoke(null);
        }

        bool CanExecuteClosePopupCommand()
        {
            return true;
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            const string key = "DownloadSubjectCodes";
            var result = parameters.TryGetValue(key, out _downloadSubjectCodes);
            if (result)
            {
                // Thực hiện Search với tham số rỗng. Lấy 15 kết quả đầu tiên.
                InternalSearch("");
            }
            else
            {
                throw new ArgumentException($"Key cannot be found in parameters, key={key}");
            }
        }

        private readonly IndexBuilder _indexBuilder;
        private readonly IUnitOfWork _unitOfWork;
        public SearchSubjectViewModel(IndexBuilder indexBuilder, IUnitOfWork unitOfWork)
        {
            _indexBuilder = indexBuilder;
            _unitOfWork = unitOfWork;
        }
    }
}
