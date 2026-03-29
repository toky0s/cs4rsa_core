using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using Cs4rsa.Database.DataProviders;
using Cs4rsa.Service.Notification.Models;

namespace Cs4rsa.Service.Notification
{
    public class NotificationService : INotificationService, INotifyPropertyChanged, IDisposable
    {
        private ObservableCollection<NotificationModel> _notifications;
        public ObservableCollection<NotificationModel> Notifications
        {
            get => _notifications;
            set
            {
                _notifications = value;
                OnPropertyChanged();
            }
        }

        private readonly BlockingCollection<NotificationModel> _queue;
        private readonly string _connectionString;
        private readonly CancellationTokenSource _cts;
        private readonly object _collectionLock = new object();
        private readonly Task _dbTask;
        private readonly RawSql _rawSql;

        public event PropertyChangedEventHandler PropertyChanged;

        public NotificationService() 
            : this(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "cs4rsa", "notification.db"))
        {
        }

        public NotificationService(string dbPath)
        {
            _notifications = new ObservableCollection<NotificationModel>();
            _queue = new BlockingCollection<NotificationModel>();
            _cts = new CancellationTokenSource();

            // Cho phép cập nhật ObservableCollection ở background thread
            BindingOperations.EnableCollectionSynchronization(_notifications, _collectionLock);

            var directory = Path.GetDirectoryName(dbPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            _connectionString = $"Data Source={dbPath};Version=3;";
            _rawSql = new RawSql(_connectionString);
            InitializeDatabase();
            LoadHistory();

            // Khởi chạy task chạy ngầm xử lý lưu sqlite
            _dbTask = Task.Factory.StartNew(ProcessQueue, _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public void ShowNotification(string message, string title = "Thông báo")
        {
            var notification = new NotificationModel
            {
                Title = title,
                Message = message,
                CreatedDate = DateTime.Now,
                IsRead = false
            };

            // Non-blocking UI add
            lock (_collectionLock)
            {
                _notifications.Insert(0, notification);
            }

            // Đẩy vào queue để lưu background
            if (!_queue.IsAddingCompleted)
            {
                _queue.Add(notification);
            }
        }

        private void InitializeDatabase()
        {
            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Notifications (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT,
                    Message TEXT,
                    CreatedDate DATETIME,
                    IsRead BOOLEAN
                )";
            _rawSql.ExecNonQuery(createTableQuery);
        }

        private void LoadHistory()
        {
            string selectQuery = "SELECT Id, Title, Message, CreatedDate, IsRead FROM Notifications ORDER BY CreatedDate DESC";
            var notifications = _rawSql.ExecReader(selectQuery, reader => new NotificationModel
            {
                Id = Convert.ToInt32(reader["Id"]),
                Title = reader["Title"].ToString(),
                Message = reader["Message"].ToString(),
                CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                IsRead = Convert.ToBoolean(reader["IsRead"])
            });

            foreach (var notification in notifications)
            {
                _notifications.Add(notification);
            }
        }

        private void ProcessQueue()
        {
            try
            {
                // Consume from queue thread-safe and non-blocking until complete
                foreach (var notification in _queue.GetConsumingEnumerable(_cts.Token))
                {
                    string insertQuery = "INSERT INTO Notifications (Title, Message, CreatedDate, IsRead) VALUES (@Title, @Message, @CreatedDate, @IsRead)";
                    var parameters = new Dictionary<string, object>
                    {
                        { "@Title", notification.Title },
                        { "@Message", notification.Message },
                        { "@CreatedDate", notification.CreatedDate },
                        { "@IsRead", notification.IsRead }
                    };

                    _rawSql.ExecNonQuery(insertQuery, parameters);

                    // Cập nhật lại ID từ sqlite sau khi lưu để đồng bộ
                    var idObj = _rawSql.ExecScalar<long>("SELECT last_insert_rowid()", 0);
                    if (idObj > 0)
                    {
                        notification.Id = Convert.ToInt32(idObj);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Break task naturally when cancelled
            }
            catch (Exception)
            {
                // Can be extended with logging
            }
        }

        public void Dispose()
        {
            if (!_cts.IsCancellationRequested)
            {
                _queue.CompleteAdding();
                _cts.Cancel();
                try
                {
                    _dbTask?.Wait(500); // Đợi tối đa 500ms cho db flow complete
                }
                catch { }
            }
            _queue.Dispose();
            _cts.Dispose();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
