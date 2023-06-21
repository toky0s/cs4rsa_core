/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CwebizAPI.Share.Validations;

/// <summary>
/// Validate rằng trường email là mail DTU.
/// </summary>
/// <remarks>
/// Ví dụ:
/// Hợp lệ: truongaxin@dtu.edu.vn
/// Không hợp lệ: truongaxin@dut.edu.vn
///
/// Created Date: 21/06/2023
/// Modified Date: 21/06/2023
/// Author: Truong A Xin
/// </remarks>
public class IsDtuEmail : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        string? strValue = value as string;
        if (string.IsNullOrEmpty(strValue)) return true;
        Regex dtuEmailRegex = new(@"^[\w-\.]+@(dtu.edu.vn)$");
        return dtuEmailRegex.IsMatch(strValue);
    }
}