using cs4rsa.Models;
using System;
using System.Collections.Generic;

namespace cs4rsa.Database
{
    public class ColorGenerator
    {
        private static readonly List<string> _excludeColors = new List<string>()
        {
            "#ffffff",
            "#111111"
        };

        public static string GenerateColor()
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

        private static bool IsHasColor(string color)
        {
            return Cs4rsaDataView.IsHaveSubjectColor(color);
        }
    }
}
