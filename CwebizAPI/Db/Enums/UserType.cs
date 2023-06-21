/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

namespace CwebizAPI.Db.Enums;

/// <summary>
/// Đại diện cho loại người dùng trong hệ thống
/// - Student: Sinh viên thông thường
/// - Trial: Tài khoản dùng thử, giới hạn một số tính năng
/// - Admin: Quản trị viên
/// </summary>
/// <remarks>
/// Created Date: 21/06/2023
/// Modified Date: 21/06/2023
/// Author: Truong A Xin
/// </remarks>
public enum UserType
{
    /// <summary>
    /// Sinh viên
    /// </summary>
    Student = 1,
    /// <summary>
    /// Dùng thử
    /// </summary>
    Trial = 2,
    /// <summary>
    /// Quản trị viên
    /// </summary>
    Admin = 3,
}