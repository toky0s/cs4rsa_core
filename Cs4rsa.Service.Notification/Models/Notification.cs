using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsa.Service.Notification.Models
{
    public class Notification
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string FromAction { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
