using ConflictService.Models;

using cs4rsa_core.BaseClasses;

using System.Collections.Generic;

namespace cs4rsa_core.Messages
{
    /// <summary>
    /// Thông báo có sự thay đổi về danh sách các xung đột thời gian của một
    /// </summary>
    class ConflictCollectionChangeMessage : Cs4rsaMessage
    {
        public new List<ConflictModel> Source;
        public ConflictCollectionChangeMessage(List<ConflictModel> source) : base(source)
        {
            Source = source;
        }
    }
}
