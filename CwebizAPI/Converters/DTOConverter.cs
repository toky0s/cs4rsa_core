/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using CwebizAPI.Share.Database.Models;
using CwebizAPI.Share.DTOs.Responses;

namespace CwebizAPI.Converters
{
    public static class DtoConverter
    {
        /// <summary>
        /// Chuyển đổi Keyword thành DtoRpKeyword.
        /// </summary>
        /// <param name="keyword">Keyword.</param>
        /// <returns>DtoRpKeyword.</returns>
        private static DtoRpKeyword ToDtoKeyword(this Keyword keyword)
        {
            return new DtoRpKeyword
            {
                Id = keyword.Id,
                Keyword1 = keyword.Keyword1,
                CourseId = keyword.CourseId,
                Color = keyword.Color,
                SubjectName = keyword.SubjectName
            };
        }

        /// <summary>
        /// Chuyển đổi Discipline thành DtoRpDiscipline
        /// </summary>
        /// <param name="discipline">Discipline</param>
        /// <returns>DtoRpDiscipline</returns>
        private static DtoRpDiscipline ToDtoDiscipline(this Discipline discipline)
        {
            return new DtoRpDiscipline
            {
                Id = discipline.Id,
                Name = discipline.Name
            };
        }

        /// <summary>
        /// Chuyển đổi danh sách Keyword thành danh sách DtoRpKeyword.
        /// </summary>
        /// <param name="keywords">Danh sách Keyword.</param>
        /// <returns>Danh sách DtoRpKeyword.</returns>
        public static IEnumerable<DtoRpKeyword> ToDtoKeywords(this IEnumerable<Keyword> keywords)
        {
            return keywords.Select(keyword => keyword.ToDtoKeyword());
        }

        /// <summary>
        /// Chuyển đổi danh sách Discipline thành DtoRpDiscipline.
        /// </summary>
        /// <param name="disciplines">Danh sách Discipline.</param>
        /// <returns>Danh sách DtoRpDiscipline.</returns>
        public static IEnumerable<DtoRpDiscipline> ToDtoDisciplines(this IEnumerable<Discipline> disciplines)
        {
            return disciplines.Select(discipline => discipline.ToDtoDiscipline());
        }
    }
}
