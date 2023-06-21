/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

namespace CwebizAPI.Crawlers.StudentCrawlerSvc.Models;

/// <summary>
/// Thông tin mã ngành. 
/// </summary>
/// <remarks>
/// Created Date: 21/06/2023
/// Modified Date: 21/06/2023
/// Author: Truong A Xin
/// </remarks>
public class CrawledCurriculum
{
    public int CurriculumId { get; set; }
    public string? Name { get; set; }
}