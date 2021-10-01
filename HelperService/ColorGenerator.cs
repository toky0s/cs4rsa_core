using Cs4rsaDatabaseService.DataProviders;
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

        private Cs4rsaDbContext _cs4rsaDbContext;

        public ColorGenerator(Cs4rsaDbContext cs4rsaDbContext)
        {
            _cs4rsaDbContext = cs4rsaDbContext;
        }

        public string GetColor(int courseId)
        {
            Keyword keyword = _cs4rsaDbContext.Keywords.Where(keyword => keyword.CourseId == courseId).FirstOrDefault();
            return keyword.Color;
        }

        public string GetColorWithSubjectCode(string subjectCode)
        {
            Keyword keyword = (from discipline in _cs4rsaDbContext.Disciplines
                                             from kw in _cs4rsaDbContext.Keywords
                                             where discipline.Name + " " + kw.Keyword1 == subjectCode
                                             select kw).FirstOrDefault();
            return keyword.Color;
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
            return _cs4rsaDbContext.Keywords.Where(kw => kw.Color == color).Any();
        }
    }
}
