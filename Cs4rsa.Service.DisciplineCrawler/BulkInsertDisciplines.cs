using Cs4rsa.Utils;

using System.Collections.Generic;
using System.Text;

namespace Cs4rsa.Service.DisciplineCrawler
{
    public class BulkInsertDisciplines
    {
        public static string GetBulkInsertSql(List<Discipline> disciplines)
        {
            StringBuilder sbDiscipline = new StringBuilder();
            sbDiscipline.AppendLine("INSERT INTO Disciplines");
            sbDiscipline.AppendLine("VALUES");

            StringBuilder sbKeyword = new StringBuilder();
            sbKeyword.AppendLine("INSERT INTO Keywords");
            sbKeyword.AppendLine("VALUES");

            int disciplineCount = disciplines.Count;
            int kwId = 1;
            for (int i = 0; i < disciplineCount; i++)
            {
                sbDiscipline.AppendLine($"({i + 1}, '{disciplines[i].Name}'),");
                foreach (Keyword keyword in disciplines[i].Keywords)
                {
                    string color = ColorGenerator.GenerateColor();
                    sbKeyword.AppendLine($"({kwId}, '{keyword.Keyword1}', {keyword.CourseId}, '{keyword.SubjectName}', '{color}', NULL, {i + 1}),");
                    kwId++;
                }
            }

            #region Remove Commas
            RemoveLastCharAfterAppendLine(sbDiscipline);
            RemoveLastCharAfterAppendLine(sbKeyword);
            sbDiscipline.Append(';');
            sbKeyword.Append(';');
            #endregion

            return sbDiscipline.ToString() + "\n" + sbKeyword.ToString();
        }

        private static void RemoveLastCharAfterAppendLine(StringBuilder sb)
        {
            sb.Length -= 3;
        }
    }
}
