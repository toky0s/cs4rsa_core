/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using System.Security.Authentication;
using System.Security.Claims;
using CwebizAPI.Db.Enums;
using CwebizAPI.Db.Interfaces;
using CwebizAPI.Share.Database.Models;

namespace CwebizAPI.Middlewares;

/// <summary>
/// Lấy ra người dùng hiện tại và gán Role.
/// </summary>
/// <remarks>
/// Ref: https://stackoverflow.com/a/62818743/9648574
/// Create Date: 16/07/2023
/// Modified Date: 16/07/2023
/// Author: Truong A Xin
/// </remarks>
public class UserClaimsMiddleware
{
    private readonly RequestDelegate _next;

    public UserClaimsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="httpContext">HttpContext</param>
    /// <param name="unitOfWork">Được tiêm từ DI</param>
    /// <exception cref="AuthenticationException">Lỗi liên quan đến thông tin Token.</exception>
    public async Task InvokeAsync(HttpContext httpContext, IUnitOfWork unitOfWork)
    {
        if (httpContext.User.Identity is { IsAuthenticated: true })
        {
            string? userId = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId is null || !int.TryParse(userId, out int id)) throw new AuthenticationException("Thông tin token không hợp lệ");

            CwebizUser? cwebizUser = await unitOfWork.UserRepository!.GetUserById(id);
            if (cwebizUser is null) throw new AuthenticationException("Người dùng không tồn tại trong hệ thống");
            
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Role, UserRole.Of(cwebizUser.UserType).ToString())
            };

            ClaimsIdentity appIdentity = new(claims);
            httpContext.User.AddIdentity(appIdentity);
        }
        await _next(httpContext);
    }
}

public static class UserClaimsMiddlewareExtensions
{
    /// <summary>
    /// Thêm UserClaimsMiddleware vào IApplicationBuilder. Thực hiện gán Role cho người dùng sau khi Authentication thực
    /// hiện thành công.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseUserClaims(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<UserClaimsMiddleware>();
    }
}