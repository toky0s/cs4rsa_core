using cs4rsa.BaseClasses;
using cs4rsa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.Messages
{
   /// <summary>
   /// Message này được publish mỗi khi Place Conflict Model Collection được cập nhật lại.
   /// </summary>
    class PlaceConflictCollectionChangeMessage : Cs4rsaMessage
    {
        public new List<PlaceConflictFinderModel> Source;
        public PlaceConflictCollectionChangeMessage(List<PlaceConflictFinderModel> source) : base(source)
        {
            Source = source;
        }
    }
}
