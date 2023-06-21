/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using CwebizAPI.Converters;
using CwebizAPI.Db.Interfaces;
using CwebizAPI.Share.Database.Models;
using CwebizAPI.Share.DTOs.Responses;

namespace CwebizAPI.Businesses
{
    /// <summary>
    /// Business Discipline
    /// </summary>
    /// <remarks>
    /// Created Date: 21/06/2023
    /// Modified Date: 21/06/2023
    /// Author: Truong A Xin
    /// </remarks>
    public class BuDiscipline
    {
        private readonly IUnitOfWork _unitOfWork;
        public BuDiscipline(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Lấy ra danh sách tất cả các Discipline.
        /// </summary>
        /// <returns>Danh sách các Discipline.</returns>
        public async Task<IEnumerable<DtoRpDiscipline>> GetAllDtoDiscipline()
        {
            List<Discipline> dbDisciplines = await _unitOfWork.DisciplineRepository!.GetAllDiscipline();
            return dbDisciplines.ToDtoDisciplines();
        }

        /// <summary>
        /// Lấy ra danh sách các Keyword dựa vào Discipline ID.
        /// </summary>
        /// <param name="disciplineId">Discipline ID.</param>
        /// <returns>Danh sách các Keyword</returns>
        public async Task<IEnumerable<DtoRpKeyword>> GetKeywordsByDisciplineId(int disciplineId)
        {
            List<Keyword> keywords = await _unitOfWork.DisciplineRepository!.GetKeywordsByDisciplineId(disciplineId);
            return keywords.ToDtoKeywords();
        }
    }
}
