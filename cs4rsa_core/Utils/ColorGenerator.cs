using Cs4rsa.Cs4rsaDatabase.Interfaces;

using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Cs4rsa.Utils
{
    public class ColorGenerator
    {
        private static readonly List<string> _excludeColors = new()
        {
            "#ffffff",
            "#111111"
        };

        private static readonly List<string> _generatedColors = new();

        private readonly IKeywordRepository _keywordRepository;

        public ColorGenerator(IKeywordRepository keywordRepository)
        {
            _keywordRepository = keywordRepository;
        }

        public async Task<string> GetColorAsync(int courseId)
        {
            return await _keywordRepository.GetColorAsync(courseId);
        }

        public string GetColorWithSubjectCode(string subjectCode)
        {
            return _keywordRepository.GetColorWithSubjectCode(subjectCode);
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
