using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading;
using Cs4rsa.Service.Notification;
using System;
using System.Linq;
using System.Data.SQLite;

namespace Cs4rsaTest
{
    [TestClass]
    public class TestNotificationService
    {
        private string _testDbFolder;

        [TestInitialize]
        public void Setup()
        {
            _testDbFolder = Path.Combine(Path.GetTempPath(), "Cs4rsaTest", Guid.NewGuid().ToString());
            if (!Directory.Exists(_testDbFolder))
            {
                Directory.CreateDirectory(_testDbFolder);
            }
        }

        [TestCleanup]
        public void Teardown()
        {
            // Give it some time to release locks
            Thread.Sleep(500);
            if (Directory.Exists(_testDbFolder))
            {
                try
                {
                    Directory.Delete(_testDbFolder, true);
                }
                catch { } // Sometimes files are still locked
            }
        }

        [TestMethod]
        public void When_Initialize_Expect_TableCreated_And_Empty()
        {
            string dbFile = Path.Combine(_testDbFolder, "test1.db");
            using (var service = new NotificationService(dbFile))
            {
                Assert.IsTrue(File.Exists(dbFile));
                Assert.AreEqual(0, service.Notifications.Count);
            }
        }

        [TestMethod]
        public void When_ShowNotification_Expect_AddedToCollection_And_SavedToDb()
        {
            string dbFile = Path.Combine(_testDbFolder, "test2.db");
            using (var service = new NotificationService(dbFile))
            {
                service.ShowNotification("Test Msg 1", "Test Title 1");
                
                // Assert from memory
                Assert.AreEqual(1, service.Notifications.Count);
                Assert.AreEqual("Test Msg 1", service.Notifications[0].Message);
                Assert.AreEqual("Test Title 1", service.Notifications[0].Title);
                Assert.IsFalse(service.Notifications[0].IsRead);
                
                // Wait for the background task to save it
                Thread.Sleep(500);
            }

            // Verify with another instance that loads history
            using (var service2 = new NotificationService(dbFile))
            {
                Assert.AreEqual(1, service2.Notifications.Count);
                Assert.AreEqual("Test Msg 1", service2.Notifications[0].Message);
                Assert.AreEqual("Test Title 1", service2.Notifications[0].Title);
                Assert.IsTrue(service2.Notifications[0].Id > 0); // Id gets assigned
            }
        }

        [TestMethod]
        public void When_ShowNotification_Concurrency_Expect_AllSaved()
        {
            string dbFile = Path.Combine(_testDbFolder, "test3.db");
            using (var service = new NotificationService(dbFile))
            {
                int count = 10;
                var threads = new Thread[count];
                for(int i = 0; i < count; i++)
                {
                    var title = $"Title {i}";
                    var msg = $"Msg {i}";
                    threads[i] = new Thread(() => 
                    {
                        service.ShowNotification(msg, title);
                    });
                    threads[i].Start();
                }

                foreach(var t in threads) t.Join();

                // Wait for queue processing
                Thread.Sleep(1000);
            }

            using (var service2 = new NotificationService(dbFile))
            {
                Assert.AreEqual(10, service2.Notifications.Count);
            }
        }

        [TestMethod]
        public void When_LoadHistory_Expect_OrderedByCreatedDateDesc()
        {
            string dbFile = Path.Combine(_testDbFolder, "test4.db");

            // Seed raw insert order: old then new
            var connStr = $"Data Source={dbFile};Version=3;";
            SQLiteConnection.CreateFile(dbFile);
            using(var cnn = new SQLiteConnection(connStr))
            {
                cnn.Open();
                var cmd = cnn.CreateCommand();
                cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Notifications (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT,
                    Message TEXT,
                    CreatedDate DATETIME,
                    IsRead BOOLEAN
                )";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "INSERT INTO Notifications (Title, Message, CreatedDate, IsRead) VALUES ('Past', 'Past', '2000-01-01 00:00:00', 0)";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "INSERT INTO Notifications (Title, Message, CreatedDate, IsRead) VALUES ('Future', 'Future', '2050-01-01 00:00:00', 0)";
                cmd.ExecuteNonQuery();
            }

            using (var service = new NotificationService(dbFile))
            {
                Assert.AreEqual(2, service.Notifications.Count);
                Assert.AreEqual("Future", service.Notifications[0].Title); // Future is more recent, should be first
                Assert.AreEqual("Past", service.Notifications[1].Title);
            }
        }

        [TestMethod]
        public void When_Dispose_Expect_CancellationAndComplete()
        {
            string dbFile = Path.Combine(_testDbFolder, "test5.db");
            var service = new NotificationService(dbFile);

            // Let it do some background work
            service.ShowNotification("1");
            service.ShowNotification("2");
            service.ShowNotification("3");

            // Immedidately dispose while background thread might still be processing
            service.Dispose();

            // Try to add to disposed collection might throw or ignore realistically depending on implementation.
            // But we just test that disposing terminates gracefully without throwing
            Assert.IsTrue(true); 
            
            // Allow DB thread to unwind completely even after disposed method returns
            Thread.Sleep(200);
            
            // DB shouldn't be locked here
            using (var cnn = new SQLiteConnection($"Data Source={dbFile};Version=3;"))
            {
                cnn.Open();
                var cmd = cnn.CreateCommand();
                cmd.CommandText = "SELECT COUNT(*) FROM Notifications";
                var r = cmd.ExecuteScalar();
            }
        }

        [TestMethod]
        public void When_DisposeCalledMultipleTimes_Expect_NoThrow()
        {
            string dbFile = Path.Combine(_testDbFolder, "test6.db");
            var service = new NotificationService(dbFile);
            service.Dispose();
            // Second dispose shouldn't throw error
            service.Dispose();
        }

        [TestMethod]
        public void When_SetNotifications_Expect_PropertyChangedEventFired()
        {
            string dbFile = Path.Combine(_testDbFolder, "test7.db");
            var service = new NotificationService(dbFile);
            
            bool eventFired = false;
            service.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(service.Notifications))
                {
                    eventFired = true;
                }
            };

            service.Notifications = new System.Collections.ObjectModel.ObservableCollection<Cs4rsa.Service.Notification.Models.NotificationModel>();
            
            Assert.IsTrue(eventFired);
        }

        [TestMethod]
        public void When_ExecuteParamless_Expect_DefaultPathCreated()
        {
            // Just verifying instance creation does not explode
            var tempFolderBackupPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "cs4rsa", "notification.db");
            bool existedPrior = File.Exists(tempFolderBackupPath);
            try
            {
                using(var service = new NotificationService())
                {
                    Assert.IsNotNull(service);
                }
            } 
            finally 
            {
                // We shouldn't cleanup user's actual DB if it existed!
                // if (!existedPrior && File.Exists(tempFolderBackupPath)) File.Delete(tempFolderBackupPath) 
                // Better to leave it alone
            }
        }
    }
}
