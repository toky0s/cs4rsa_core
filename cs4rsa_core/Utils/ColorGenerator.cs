using cs4rsa_core.Cs4rsaDatabase.Interfaces;

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace cs4rsa_core.Utils
{
    public class ColorGenerator
    {
        private static readonly List<string> _excludeColors = new()
        {
            "#ffffff",
            "#111111"
        };

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

        public string GenerateColor()
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
            while (IsHasColor(color) || _excludeColors.Contains(color));
            return color;
        }

        private bool IsHasColor(string color)
        {
            return _keywordRepository.IsHasColor(color);
        }
    }
}
