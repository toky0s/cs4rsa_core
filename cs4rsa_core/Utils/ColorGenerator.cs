using Cs4rsa.Cs4rsaDatabase.Interfaces;

using System.Collections.Generic;
using System.Security.Cryptography;

namespace Cs4rsa.Utils
{
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

        public string GetColor(int courseId)
        {
            return _unitOfWork.Keywords.GetColor(courseId);
        }

        public string GetColorWithSubjectCode(string subjectCode)
        {
            return _unitOfWork.Keywords.GetColorWithSubjectCode(subjectCode);
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
