using Microsoft.Extensions.Logging;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsa.Service.Notification
{
    public class NotificationEventArgs
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime CreatedOn { get; set; }
        public string FromAction { get; set; }
    }

    public class NotificationEvent: PubSubEvent<NotificationEventArgs>
    {
    }

    public class NotificationService : INotificationService
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            IEventAggregator eventAggregator,
            ILogger<NotificationService> logger
        )
        {
            _eventAggregator = eventAggregator;
            _logger = logger;
        }

        public void SendNotification(string title, string message, string fromAction)
        {
            _logger.LogInformation( title, message );
            _eventAggregator.GetEvent<NotificationEvent>().Publish(new NotificationEventArgs
            {
                Title = title,
                Message = message,
                FromAction = fromAction,
                CreatedOn = DateTime.Now
            });
        }
    }
}
