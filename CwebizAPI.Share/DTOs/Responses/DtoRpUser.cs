/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

namespace CwebizAPI.Share.DTOs.Responses;

/// <summary>
/// DtoRpUser
/// 
/// Thông tin người dùng
/// </summary>
/// <remarks>
/// Created Date: 16/07/2023
/// Modified Date: 16/07/2023
/// Author: Truong A Xin
/// </remarks>
public class DtoRpUser
{
    /// <summary>
    /// Mã sinh viên
    /// </summary>
    public string StudentId { get; set; }
    /// <summary>
    /// Tên đăng nhập
    /// </summary>
    public string Username { get; set; }
    /// <summary>
    /// Quyền người dùng
    /// </summary>
    public string UserRole { get; set; }
    /// <summary>
    /// Tên người dùng
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Ngày sinh
    /// </summary>
    public DateTime? BirthDay { get; set; }
    /// <summary>
    /// Số chứng minh thư
    /// </summary>
    public string? Cmnd { get; set; }
    /// <summary>
    /// Email
    /// </summary>
    public string? Email { get; set; }
    /// <summary>
    /// Số điện thoại
    /// </summary>
    public string? PhoneNumber { get; set; }
    /// <summary>
    /// Địa chỉ
    /// </summary>
    public string? Address { get; set; }
    /// <summary>
    /// Hình ảnh đại diện
    /// </summary>
    public string? AvatarImgPath { get; set; }
}