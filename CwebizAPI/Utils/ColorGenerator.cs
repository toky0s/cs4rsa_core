/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using CwebizAPI.Db.Interfaces;

using System.Security.Cryptography;

namespace CwebizAPI.Utils
{
    /// <summary>
    /// Color Generator
    /// Trình tạo màu sắc.
    /// </summary>
    public class ColorGenerator
    {
        /// <summary>
        /// Danh sách các màu sắc bị loại trừ trong quá trình 
        /// tạo ngẫu nhiên màu sắc.
        /// </summary>
        private static readonly List<string> _excludeColors = new()
        {
            "#ffffff",
            "#111111"
        };

        private static readonly List<string> _generatedColors = new();

        private readonly IUnitOfWork _unitOfWork;

        public ColorGenerator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> GetColor(string courseId)
        {
            return await _unitOfWork.DisciplineRepository.GetColor(courseId);
        }

        public async Task<string> GetColorWithSubjectCode(string subjectCode)
        {
            return await _unitOfWork.DisciplineRepository!.GetColorWithSubjectCode(subjectCode);
        }

        public static string GenerateColor()
        {
            RandomNumberGenerator.Create();
            string color;
            do
            {
                int red = (RandomNumberGenerator.GetInt32(256) + 255) / 2;
                int green = (RandomNumberGenerator.GetInt32(256) + 255) / 2;
                int blue = (RandomNumberGenerator.GetInt32(256) + 255) / 2;
                color = $"#{red:X2}{green:X2}{blue:X2}";
            }
            while (_excludeColors.Contains(color) || _generatedColors.Contains(color));
            _generatedColors.Add(color);
            return color;
        }
    }
}
