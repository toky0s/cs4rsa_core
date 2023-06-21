/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using CwebizAPI.Crawlers.StudentCrawlerSvc.Models;
using CwebizAPI.Share.Database.Models;

namespace CwebizAPI.Converters;

/// <summary>
/// Bộ chuyển đổi từ Model Crawler sang Model DB.
/// </summary>
/// <remarks>
/// Created Date: 16/06/2023
/// Modified Date: 16/06/2023
/// Author: Truong A Xin
/// </remarks>
public static class CrawlDbModelConverter
{
    /// <summary>
    /// Chuyển đổi sang Student Model.
    /// </summary>
    /// <param name="dtuStudent">DtuStudent</param>
    /// <returns>Student</returns>
    public static Student ToStudent(this DtuStudent dtuStudent)
    {
        return new Student()
        {
            StudentId = dtuStudent.StudentId!,
            Address = dtuStudent.Address,
            Cmnd = dtuStudent.Cmnd,
            Email = dtuStudent.Emails[0],
            Name = dtuStudent.Name!,
            BirthDay = dtuStudent.BirthDay,
            PhoneNumber = dtuStudent.PhoneNumber,
            SpecialString = dtuStudent.SpecialString!,
            AvatarImgPath = dtuStudent.AvatarImgPath
        };
    }

    /// <summary>
    /// Chuyển đổi Crawled Curriculum sang Curriculum.
    /// </summary>
    /// <param name="crawledCurriculum">CrawledCurriculum</param>
    /// <returns>Curriculum</returns>
    public static Curriculum ToCurriculum(this CrawledCurriculum crawledCurriculum)
    {
        return new Curriculum()
        {
            Name = crawledCurriculum.Name!,
            Id = crawledCurriculum.CurriculumId
        };
    }
}