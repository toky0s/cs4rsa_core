namespace CwebizAPI.DTOs.Responses
{
    /// <summary>
    /// Đối tượng thông báo lỗi.
    /// 
    /// Created Date: 12/07/2023
    /// Modified Date: 12/07/2023
    /// Author: Truong A Xin
    /// </summary>
    public class ErrorResponse
    {
        public string Message { get; set; }
        public long Timestamp { get; set; }

        public ErrorResponse(string message)
        {
            Message = message;
            Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
        }
    }
}