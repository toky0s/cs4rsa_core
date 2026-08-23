using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Cs4rsa.Common
{
    public class ColorGenerator
    {
        /// <summary>
        /// Danh sách các màu sắc bị loại trừ trong quá trình 
        /// tạo ngẫu nhiên màu sắc.
        /// </summary>
        private static readonly string[] _excludeColors = new string[] {
            "#ffffff",
            "#111111"
        };

        private static readonly List<string> _generatedColors = new List<string>();

        public static string GenerateColor()
        {
            RandomNumberGenerator.Create();
            string color;
            do
            {
                Random rand = new();
                var red = (rand.Next(0, 257) + 255) / 2;
                var green = (rand.Next(0, 257) + 255) / 2;
                var blue = (rand.Next(0, 257) + 255) / 2;
                color = $"#{red:X2}{green:X2}{blue:X2}";
            }
            while (_excludeColors.Contains(color) || _generatedColors.Contains(color));
            _generatedColors.Add(color);
            return color;
        }
    }
}
