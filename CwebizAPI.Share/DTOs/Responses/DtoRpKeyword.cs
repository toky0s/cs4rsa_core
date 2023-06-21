/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

namespace CwebizAPI.Share.DTOs.Responses
{
    /// <summary>
    /// DTO Response Keyword.
    /// </summary>
    /// <remarks>
    /// Created Date: 21/06/2023
    /// Modified Date: 21/06/2023
    /// Author: Truong A Xin
    /// </remarks>
    public class DtoRpKeyword
    {
        public int Id { get; set; }
        public string? Keyword1 { get; set; }
        public int? CourseId { get; set; }
        public string? SubjectName { get; set; }
        public string? Color { get; set; }
        public string? Cache { get; set; }
    }
}