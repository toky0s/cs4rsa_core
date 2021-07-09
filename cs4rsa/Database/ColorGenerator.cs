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

        public static string GetColor(string courseId)
        {
            Cs4rsaDatabase cs4RsaDatabase = Cs4rsaDatabase.GetInstance();
            string sql = $@"SELECT color from keyword WHERE course_id = {courseId}";
            return cs4RsaDatabase.GetScalar<string>(sql);
        }

        public static string GetColorWithSubjectCode(string subjectCode)
        {
            Cs4rsaDatabase cs4RsaDatabase = Cs4rsaDatabase.GetInstance();
            string sql = $@"select color from discipline, keyword
                            where discipline.name||' '||keyword1 = '{subjectCode}' 
                            AND discipline.id = keyword.discipline_id";
            return cs4RsaDatabase.GetScalar<string>(sql);
        }

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
            Cs4rsaDatabase cs4RsaDatabase = Cs4rsaDatabase.GetInstance();
            string sqlString = $@"SELECT count(color) from keyword where color = '{color}'";
            long result = cs4RsaDatabase.GetScalar<long>(sqlString);
            if (result >= 1) return true;
            return false;
        }
    }
}
