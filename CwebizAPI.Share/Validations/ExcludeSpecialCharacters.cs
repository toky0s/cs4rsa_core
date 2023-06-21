/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using System.ComponentModel.DataAnnotations;

namespace CwebizAPI.Share.Validations;

/// <summary>
/// Validate rằng trường không chứa ký tự đặc biệt
/// </summary>
/// <remarks>
/// Ví dụ:
/// Hợp lệ: username15password11
/// Không hợp lệ: user15.name$11
///
/// Created Date: 21/06/2023
/// Modified Date: 21/06/2023
/// Author: Truong A Xin
/// </remarks>
public class ExcludeSpecialCharacters : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is null) return true;
        var inputString = value.ToString();
        return inputString != null && inputString.All(char.IsLetterOrDigit);
    }
}