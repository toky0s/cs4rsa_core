/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using CwebizAPI.Converters;
using CwebizAPI.Db.Interfaces;
using CwebizAPI.DTOs.Responses;
using CwebizAPI.Share.Database.Models;

namespace CwebizAPI.Businesses;

/// <summary>
/// Business Course.
/// 
/// Created Date: 02/08/2023
/// Modified Date: 02/08/2023
/// Author: Truong A Xin
/// </summary>
public class BuCourse
{
    private readonly IUnitOfWork _unitOfWork;
    
    public BuCourse(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    
    public async Task<DtoRpCourse> GetLatestCourse()
    {
        Course? course = await _unitOfWork.DisciplineRepository.GetLatestCourse();
        if (course is null)
        {
            throw new BadHttpRequestException("Không tồn tại thông tin môn học mới nhất");
        }

        return course.ToDtoRpCourse();
    }
}