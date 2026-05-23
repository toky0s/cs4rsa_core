using Cs4rsa.Common;

using System.Collections.Generic;
using System.Text;

namespace Cs4rsa.Service.DisciplineCrawler
{
    public class BulkInsertDisciplines
    {
        public static string GetBulkInsertSql(List<Discipline> disciplines)
        {
            var sbDiscipline = new StringBuilder();
            sbDiscipline.AppendLine("INSERT INTO Disciplines");
            sbDiscipline.AppendLine("VALUES");

            var sbKeyword = new StringBuilder();
            sbKeyword.AppendLine("INSERT INTO Keywords");
            sbKeyword.AppendLine("VALUES");

            var disciplineCount = disciplines.Count;
            var kwId = 1;
            for (var i = 0; i < disciplineCount; i++)
            {
                var disciplineId = i + 1;
                sbDiscipline.AppendLine($"({disciplineId}, '{disciplines[i].Name}'),");
                foreach (var keyword in disciplines[i].Keywords)
                {
                    var color = ColorGenerator.GenerateColor();
                    sbKeyword.AppendLine($"({kwId}, '{keyword.Keyword1}', {keyword.CourseId}, '{keyword.SubjectName}', '{color}', NULL, {disciplineId}, '{keyword.SemesterId}'),");
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
