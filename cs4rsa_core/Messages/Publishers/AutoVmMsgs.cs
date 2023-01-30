using CommunityToolkit.Mvvm.Messaging.Messages;

using Cs4rsa.Models;

using System.Collections.Generic;

namespace Cs4rsa.Messages.Publishers
{
    internal sealed class AutoVmMsgs
    {
        /// <summary>
        /// Hiển thị lớp đã chọn lên lịch
        /// </summary>
        internal sealed class ShowOnSimuMsg : ValueChangedMessage<CombinationModel>
        {
            public ShowOnSimuMsg(CombinationModel value) : base(value)
            {
            }
        }

        internal sealed class AddTreeItemMsg : ValueChangedMessage<ProgramSubjectModel>
        {
            public AddTreeItemMsg(ProgramSubjectModel value) : base(value)
            {
            }
        }


        /// <summary>
        /// Thêm vào tập kết quả các kết hợp
        /// </summary>
        internal sealed class AddCombinationsMsg : ValueChangedMessage<List<CombinationModel>>
        {
            public AddCombinationsMsg(List<CombinationModel> value) : base(value)
            {

            }
        }

        /// <summary>
        /// Lưu bộ sắp xếp vào kho
        /// </summary>
        internal sealed class SaveStoreMsg : ValueChangedMessage<IEnumerable<CombinationModel>>
        {
            public SaveStoreMsg(IEnumerable<CombinationModel> value) : base(value)
            {

            }
        }
    }
}
