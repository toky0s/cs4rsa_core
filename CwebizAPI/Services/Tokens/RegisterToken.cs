/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

namespace CwebizAPI.Services.Tokens;

/// <summary>
/// RegisterToken
/// </summary>
/// <remarks>
/// Payload được nhét trong JWT respose lúc thực hiện đăng ký.
/// </remarks>
public class RegisterToken
{
    /// <summary>
    /// Mã sinh viên.
    /// </summary>
    public string? StudentId { get; init; }
}