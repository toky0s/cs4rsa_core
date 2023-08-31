/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

namespace CwebizAPI.DTOs.Responses;

/// <summary>
/// Thông tin người dùng trả về sau khi liên kết thành công.
/// </summary>
/// <remarks>
/// Created Date: 21/06/2023
/// Modified Date: 21/06/2023
/// Author: Truong A Xin
/// </remarks>
public class DtoRpLinkStudent
{
    public string? StudentId { get; set; }
    public string? UserName { get; set; }
}