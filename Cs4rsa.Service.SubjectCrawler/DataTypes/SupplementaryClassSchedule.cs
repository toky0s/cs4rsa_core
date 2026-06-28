using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsa.Service.SubjectCrawler.DataTypes
{
    public class SupplementaryClassSchedule
    {
        private DateTime _from;
        private DateTime _to;
        private string _room;
        private string _place;

        public DateTime From { get => _from; }
        public DateTime To { get => _to; }
        public string Room { get => _room; }
        public string Place { get => _place; }

        public SupplementaryClassSchedule(DateTime from, DateTime to, string room, string place)
        {
            _from = from;
            _to = to;
            _room = room;
            _place = place;
        }
    }
}
