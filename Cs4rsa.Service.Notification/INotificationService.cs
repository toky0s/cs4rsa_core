using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cs4rsa.Service.Notification.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Cs4rsa.Service.Notification
{
    public interface INotificationService : INotifyPropertyChanged
    {
        /// <summary>
        /// Danh sách các thông báo.
        /// </summary>
        ObservableCollection<NotificationModel> Notifications { get; set; }

        /// <summary>
        /// Hiển thị một thông báo với nội dung và tiêu đề tùy chỉnh.
        /// </summary>
        /// <param name="message">Nội dung của thông báo.</param>
        /// <param name="title">Tiêu đề của thông báo (tùy chọn).</param>
        void ShowNotification(string message, string title = "Thông báo");
    }
}
