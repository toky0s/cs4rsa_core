/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using CwebizAPI.Db.Enums;
using CwebizAPI.Db.Interfaces;
using CwebizAPI.Share.Database.Models;
using CwebizAPI.Share.DTOs.Responses;

namespace CwebizAPI.Businesses;

/// <summary>
/// Lớp xử lý nghiệp vụ Người dùng.
/// </summary>
/// <remarks>
/// Created Date: 16/07/2023
/// Modified Date: 16/07/2023
/// Author: Truong A Xin
/// </remarks>
public class BuUser
{
    private readonly IUnitOfWork _unitOfWork;
    public BuUser(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Lấy ra thông tin người dùng hiện tại.
    /// </summary>
    /// <param name="userId">Mã người dùng.</param>
    /// <returns>DtoRpUser</returns>
    /// <exception cref="BadHttpRequestException"></exception>
    public async Task<DtoRpUser> GetUser(int userId)
    {
        CwebizUser? cwebizUser = await _unitOfWork.UserRepository!.GetUserById(userId);
        if (cwebizUser is null) throw new BadHttpRequestException($"Không tìm thấy thông tin người dùng với id là {userId}");
        DtoRpUser dtoRpUser = new()
        {
            Username = cwebizUser.Username,
            UserRole = UserRole.Of(cwebizUser.UserType).ToString()
        };
        if (cwebizUser.StudentId is null)
        {
            return dtoRpUser;
        }
        // Trả về thêm thông tin Student nếu có liên kết.
        Student student = await _unitOfWork.StudentRepository!.GetByStudentId(cwebizUser.StudentId);
        dtoRpUser.StudentId = student.StudentId;
        dtoRpUser.Name = student.Name;
        dtoRpUser.Address = student.Address;
        dtoRpUser.Cmnd = student.Cmnd;
        dtoRpUser.Email = student.Email;
        dtoRpUser.BirthDay = student.BirthDay;
        dtoRpUser.AvatarImgPath = student.AvatarImgPath;
        dtoRpUser.PhoneNumber = student.PhoneNumber;
        return dtoRpUser;
    }
}