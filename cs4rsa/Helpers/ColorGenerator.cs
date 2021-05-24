using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.Helpers
{
    class ColorGenerator
    {
        private static List<string> Colors = new List<string>();
        public static string GetColor()
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
            while (Colors.Contains(color));
            return color;
        }
    }
}
