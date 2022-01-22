using DisciplineCrawlerDLL.Models;

namespace DisciplineCrawlerDLL.Utils
{
    public class ColorGen
    {
        private static readonly List<string> _excludeColors = new()
        {
            "#ffffff",
            "#111111"
        };

        private readonly List<Discipline> _disciplines;

        public ColorGen(List<Discipline> disciplines)
        {
            _disciplines = disciplines;
        }

        public string GetColorAsync(int courseId)
        {
            foreach (Discipline discipline in _disciplines)
            {
                Keyword? keyword = discipline.Keywords
                    .Where(keyword => keyword.CourseId == courseId)
                    .FirstOrDefault();
                if (keyword != null)
                {
                    return keyword.Color;
                }
            }
            return "";
        }

        public string GenerateColor()
        {
            string color;
            do
            {
                Random random = new();
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
            foreach (Discipline discipline in _disciplines)
            {
                Keyword? keyword = discipline.Keywords.FirstOrDefault(keyword => keyword.Color == color);
                if (keyword == null)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
