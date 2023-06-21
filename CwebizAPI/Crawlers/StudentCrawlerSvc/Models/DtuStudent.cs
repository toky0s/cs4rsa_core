/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

namespace CwebizAPI.Crawlers.StudentCrawlerSvc.Models;

/// <summary>
/// Thông tin Sinh viên
/// </summary>
/// <remarks>
/// Created Date: 21/06/2023
/// Modified Date: 21/06/2023
/// Author: Truong A Xin
/// </remarks>
public class DtuStudent
{
    /// <summary>
    /// Mã sinh viên
    /// </summary>
    public string? StudentId { get; set; }

    /// <summary>
    /// Mã hash
    /// </summary>
    public string? SpecialString { get; set; }

    /// <summary>
    /// Tên sinh viên
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Ngày tháng năm sinh
    /// </summary>
    public DateTime BirthDay { get; set; }

    /// <summary>
    /// Số chứng minh nhân dân
    /// </summary>
    public string? Cmnd { get; set; }

    /// <summary>
    /// Địa chỉ Email
    /// </summary>
    public string[] Emails { get; set; }

    /// <summary>
    /// Số điện thoại
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Địa chỉ
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Đường dẫn tới ảnh hồ sơ
    /// </summary>
    public string? AvatarImgPath { get; set; }
}