using CommunityToolkit.Mvvm.Messaging.Messages;

using Cs4rsa.Cs4rsaDatabase.Models;

namespace Cs4rsa.Messages.Publishers.Dialogs
{
    internal sealed class AccountVmMsgs
    {
        /// <summary>
        /// Xoá Student
        /// </summary>
        internal sealed class DelStudentMsg : ValueChangedMessage<string>
        {
            public DelStudentMsg(string studentId) : base(studentId)
            {

            }
        }

        /// <summary>
        /// Hoàn tác xoá Student
        /// </summary>
        internal sealed class UndoDelStudentMsg : ValueChangedMessage<Student>
        {
            public UndoDelStudentMsg(Student value) : base(value)
            {

            }
        }
    }
}
