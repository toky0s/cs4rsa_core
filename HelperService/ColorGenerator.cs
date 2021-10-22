using Cs4rsaDatabaseService.DataProviders;
using Cs4rsaDatabaseService.Interfaces;
using Cs4rsaDatabaseService.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HelperService
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

        public string GetColor(int courseId)
        {
            return _keywordRepository.GetColor(courseId);
        }

        public string GetColorWithSubjectCode(string subjectCode)
        {
            return _keywordRepository.GetColorWithSubjectCode(subjectCode);
        }

        public string GenerateColor()
        {
            string color;
            do
            {
                Random random = new Random();
                int red = (random.Next(256) + 255) / 2;
                int green = (random.Next(256) + 255) / 2;
                int blue = (random.Next(256) + 255) / 2;
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
