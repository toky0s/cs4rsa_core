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
public class UserRole
{
    /// <summary>
    /// Sinh viên
    /// </summary>
    public static readonly UserRole Student = new("Student", 1);
    /// <summary>
    /// Dùng thử
    /// </summary>
    public static readonly UserRole Trial = new("Trial", 2);
    /// <summary>
    /// Quản trị viên
    /// </summary>
    public static readonly UserRole Admin = new("Admin", 3);

    private static readonly UserRole[] Roles = { Student, Trial, Admin };
    private readonly string _value;
    public readonly int DbValue;
    private UserRole(string value, int dbValue)
    {
        _value = value;
        DbValue = dbValue;
    }

    public static UserRole Of(int value)
    {
        UserRole? role = Roles.FirstOrDefault(r => r.DbValue == value);
        return role ?? Trial;
    }

    public override string ToString()
    {
        return _value;
    }
}