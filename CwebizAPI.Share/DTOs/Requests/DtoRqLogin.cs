/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using System.ComponentModel.DataAnnotations;

namespace CwebizAPI.Share.DTOs.Requests;

/// <summary>
/// DTO Request Login
/// Thông tin đăng nhập
/// </summary>
/// Created Date: 16/07/2023
/// Modified Date: 16/07/2023
/// Author: Truong A Xin
public class DtoRqLogin
{
    [Required]
    public string? Username { get; set; }
    
    [Required]
    public string? Password { get; set; }
}