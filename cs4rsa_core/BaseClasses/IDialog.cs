namespace cs4rsa_core.BaseClasses
{
    /// <summary>
    /// Mọi Dialog trong Cs4rsa phải implement
    /// interface này nhằm xác định xem Dialog
    /// đó có thể Close khi Click ra bên ngoài
    /// hay khum :D.
    /// </summary>
    public interface IDialog
    {
        public bool IsCloseOnClickAway();
    }
}
