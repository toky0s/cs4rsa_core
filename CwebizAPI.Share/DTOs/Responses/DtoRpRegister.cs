/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

namespace CwebizAPI.Share.DTOs.Responses
{
    /// <summary>
    /// DTO Response Register.
    /// 
    /// Thông tin trả về khi đăng ký thành
    /// công tài khoản cho một sinh viên.
    /// </summary>
    /// <remarks>
    /// Created Date: 21/06/2023
    /// Modified Date: 21/06/2023
    /// Author: Truong A Xin
    /// </remarks>
    public class DtoRpRegister
    {
        public string? JwtRegister { get; set; }
    }
}
