using Cs4rsa.Common;

using System.Collections.Generic;
using System.Text;

namespace Cs4rsa.Service.DisciplineCrawler
{
    public class BulkInsertDisciplinesPg
    {
        public static string GetBulkInsertSql(
            string yearInfo, string yearValue,
            string semesterInfo, string semesterValue,
            int currentDisciplineId, int currentKeywordId,
            List<Discipline> disciplines)
        {
            #region Declare variables
            var sbCourse = new StringBuilder();
            sbCourse.AppendLine("DO $$");
            sbCourse.AppendLine("DECLARE");
            sbCourse.AppendLine("   new_course_id INT;");
            sbCourse.AppendLine("   count_course_by_semestervalue INT;");
            sbCourse.AppendLine("   found_course_id INT;");
            sbCourse.Append("   semester_value TEXT := '").Append(semesterValue).AppendLine("';");
            sbCourse.AppendLine("BEGIN");
            #endregion

            #region Delete course by semester value to ensure that update latest data for current course, discipline and keyword
            sbCourse.AppendLine("SELECT");
            sbCourse.AppendLine("   COUNT(id), id INTO count_course_by_semestervalue, found_course_id");
            sbCourse.AppendLine("FROM course");
            sbCourse.AppendLine("WHERE semestervalue = semester_value");
            sbCourse.AppendLine("GROUP BY id;");

            sbCourse.AppendLine("IF count_course_by_semestervalue = 1 THEN");
            sbCourse.AppendLine("   RAISE NOTICE 'Found course: %, Course ID: %', count_course_by_semestervalue, found_course_id;");
            sbCourse.AppendLine("   DELETE FROM keyword");
            sbCourse.AppendLine("   WHERE keyword.disciplineid IN(");
            sbCourse.AppendLine("       SELECT id");
            sbCourse.AppendLine("       FROM discipline");
            sbCourse.AppendLine("       WHERE discipline.courseid = found_course_id");
            sbCourse.AppendLine("   );");

            sbCourse.AppendLine("   DELETE FROM discipline");
            sbCourse.AppendLine("   WHERE courseid = found_course_id;");
            
            sbCourse.AppendLine("   DELETE FROM course WHERE id = found_course_id;");

            sbCourse.AppendLine("ELSE");
            sbCourse.AppendLine("   RAISE NOTICE 'No course with semester value % found', semester_value;");
            sbCourse.AppendLine("END IF;");
            #endregion

            #region Insert course
            sbCourse.AppendLine("SELECT id + 1 INTO new_course_id FROM course ORDER BY id DESC LIMIT 1;");
            sbCourse.AppendLine("INSERT INTO course(id, yearinfor, yearvalue, semesterinfor, semestervalue, createddate)");
            sbCourse.Append("VALUES(new_course_id, '")
                .Append(yearInfo)
                .Append("', '")
                .Append(yearValue)
                .Append("', '")
                .Append(semesterInfo)
                .Append("', '")
                .Append(semesterValue)
                .AppendLine("', CURRENT_TIMESTAMP);");
            #endregion

            #region Insert disciplines and keywords
            var sbDiscipline = new StringBuilder();
            sbDiscipline.AppendLine("INSERT INTO discipline(id, name, courseid)");
            sbDiscipline.AppendLine("VALUES");

            var sbKeyword = new StringBuilder();
            sbKeyword.AppendLine("INSERT INTO keyword(id, keyword1, courseid, subjectname, color, html, disciplineid)");
            sbKeyword.AppendLine("VALUES");

            var disciplineCount = disciplines.Count;
            var kwId = currentKeywordId;
            
            for (var i = 0; i < disciplines.Count; i++)
            {
                disciplineCount++;
                sbDiscipline.AppendLine($"({disciplineCount}, '{disciplines[i].Name}', new_course_id),");
                foreach (var keyword in disciplines[i].Keywords)
                {
                    var color = ColorGenerator.GenerateColor();
                    kwId++;
                    sbKeyword.AppendLine($"({kwId}, '{keyword.Keyword1}', {keyword.CourseId}, '{keyword.SubjectName}', '{color}', NULL, {disciplineCount}),");
                }
            }
            #endregion

            #region Remove Commas
            RemoveLastCharAfterAppendLine(sbDiscipline);
            RemoveLastCharAfterAppendLine(sbKeyword);
            sbDiscipline.AppendLine(";");
            sbKeyword.AppendLine(";");
            #endregion

            #region END
            sbKeyword.AppendLine("END $$;");
            #endregion

            return sbCourse.ToString() + 
                "\n" + sbDiscipline.ToString() + 
                "\n" + sbKeyword.ToString();
        }

        private static void RemoveLastCharAfterAppendLine(StringBuilder sb)
        {
            sb.Length -= 2;
        }
    }
}
