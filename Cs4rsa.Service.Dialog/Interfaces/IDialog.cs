namespace Cs4rsa.Service.Dialog.Interfaces
{
    /// <summary>
    /// Mọi Dialog trong Cs4rsa phải implement
    /// interface này nhằm xác định xem Dialog
    /// đó có thể Close khi Click ra bên ngoài
    /// hay khum :D.
    /// </summary>
    public interface IDialog
    {
        /// <summary>
        /// Implement phương thức này để xác định hành vi
        /// click ra ngoài để đóng Dialog.
        /// <list type="bullet">
        /// <listheader>
        /// <term>true</term>
        /// <description>Cho phép đóng dialog khi click ra bên ngoài.</description>
        /// </listheader>
        /// <item>
        /// <term>false</term>
        /// <description>Không có phép đóng dialog khi click ra bên ngoài.</description>
        /// </item>
        /// </list>
        /// </summary>
        /// <returns></returns>
        bool IsCloseOnClickAway();
    }
}
