/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using CwebizAPI.Crawlers.SubjectCrawlerSvc.Crawlers.Interfaces;
using CwebizAPI.Crawlers.SubjectCrawlerSvc.DataTypes;
using CwebizAPI.DTOs.Responses;
using CwebizAPI.Services.Interfaces;

namespace CwebizAPI.Businesses;

/// <summary>
/// Lớp xử lý nghiệp vụ môn học.
/// </summary>
/// <remarks>
/// Created Date: 25/07/2023
/// Modified Date: 25/07/2023
/// Author: Truong A Xin
/// </remarks>
public class BuSubject
{
    private readonly ISubjectCrawler _subjectCrawler;
    private readonly ISvcSubjectCvt _svcSubjectCvt;
    public BuSubject(ISubjectCrawler subjectCrawler, ISvcSubjectCvt svcSubjectCvt)
    {
        _subjectCrawler = subjectCrawler;
        _svcSubjectCvt = svcSubjectCvt;
    }

    public async Task<DtoRpSubject?> GetSubjectByDisciplineKeyword(string discipline, string keyword)
    {
        Subject? subject = await _subjectCrawler.Crawl(discipline, keyword);
        if (subject is null) throw new BadHttpRequestException($"Không tìm thấy môn học {discipline} {keyword}");
        return await _svcSubjectCvt.ToDtoRpSubject(subject);
    }

    public async Task<DtoRpSubject?> GetSubjectByCourseId(string courseId)
    {
        Subject? subject = await _subjectCrawler.Crawl(courseId);
        if (subject is null) throw new BadHttpRequestException($"Không tìm thấy môn học có Course ID là {courseId}");
        return await _svcSubjectCvt.ToDtoRpSubject(subject);
    }
}