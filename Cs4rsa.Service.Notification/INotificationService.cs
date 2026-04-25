using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsa.Service.Notification
{
    public interface INotificationService
    {
        /// <summary>
        /// Gửi một thông báo đến người dùng.
        /// </summary>
        /// <param name="title">Tiêu đề của thông báo.</param>
        /// <param name="message">Nội dung của thông báo.</param>
        /// <param name="fromAction">Hành động hoặc nguồn gốc của thông báo.</param>
        void SendNotification(string title, string message, string fromAction);
    }
}
