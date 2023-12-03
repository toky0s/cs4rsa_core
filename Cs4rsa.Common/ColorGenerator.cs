using System;
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

        public static string GenerateColor()
        {
            RandomNumberGenerator.Create();
            string color;
            do
            {
                Random rand = new();
                int red = (rand.Next(0, 257) + 255) / 2;
                int green = (rand.Next(0, 257) + 255) / 2;
                int blue = (rand.Next(0, 257) + 255) / 2;
                color = $"#{red:X2}{green:X2}{blue:X2}";
            }
            while (_excludeColors.Contains(color) || _generatedColors.Contains(color));
            _generatedColors.Add(color);
            return color;
        }
    }
}
