/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

namespace CwebizAPI.Share.DTOs.Responses;

/// <summary>
/// DTO Response Login
/// Bao gồm thông tin Token khi đăng nhập thành công.
/// </summary>
/// <remarks>
/// Created Date: 16/07/2023
/// Modified Date: 16/07/2023
/// Author: Truong A Xin
/// </remarks>
public class DtoRpLogin
{
    public string Token { get; set; }
}