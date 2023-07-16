/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using System.ComponentModel.DataAnnotations;
using CwebizAPI.Share.Validations;

namespace CwebizAPI.Share.DTOs.Requests;

/// <summary>
/// DTO Request Link Account
/// Thông tin đăng ký tài khoản hệ thống.
/// </summary>
/// <remarks>
/// Created Date: 21/06/2023
/// Modified Date: 21/06/2023
/// - 06/07/2023: Thêm trường RePassword, thêm Error Message cho Required
/// Author: Truong A Xin
/// </remarks>
public class DtoRqLinkAccount
{
    /// <summary>
    /// Tên đăng nhập
    /// </summary>
    [Required(ErrorMessage = "Đây là trường bắt buộc")]
    [MinLength(5, ErrorMessage = "Yêu cầu tối thiểu 5 ký tự")]
    [MaxLength(30, ErrorMessage = "Yêu cầu tối đa 30 ký tự")]
    [ExcludeSpecialCharacters(ErrorMessage = "Trường Username không được chưa ký tự đặc biệt")]
    public string? Username { get; set; }

    /// <summary>
    /// Mật khẩu
    /// </summary>
    [Required(ErrorMessage = "Đây là trường bắt buộc")]
    [MinLength(5, ErrorMessage = "Yêu cầu tối thiểu 5 ký tự")]
    [MaxLength(30, ErrorMessage = "Yêu cầu tối đa 30 ký tự")]
    [ExcludeSpecialCharacters(ErrorMessage = "Trường Password không được chưa ký tự đặc biệt")]
    public string? Password { get; set; }

    /// <summary>
    /// Mật khẩu
    /// </summary>
    [Required(ErrorMessage = "Đây là trường bắt buộc")]
    [MinLength(5, ErrorMessage = "Yêu cầu tối thiểu 5 ký tự")]
    [MaxLength(30, ErrorMessage = "Yêu cầu tối đa 30 ký tự")]
    [ExcludeSpecialCharacters(ErrorMessage = "Trường Password không được chưa ký tự đặc biệt")]
    [Compare("Password", ErrorMessage = "Không trùng khớp với mật khẩu cũ")]
    public string? RePassword { get; set; }

    /// <summary>
    /// Token thực hiện liên kết tài khoản sinh viên
    /// </summary>
    [Required]
    public string? JwtRegister { get; set; }
}