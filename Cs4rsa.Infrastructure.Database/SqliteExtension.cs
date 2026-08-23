using System.Text;

namespace Cs4rsa.Database
{
    internal static class SqliteExtension
    {
        /// <summary>
        /// Append line BEGIN
        /// </summary>
        /// <param name="stringBuilder">StringBuilder</param>
        /// <returns>StringBuilder</returns>
        internal static StringBuilder BeginTransaction(this StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine("BEGIN;");
            return stringBuilder;
        }

        /// <summary>
        /// Append line COMMIT
        /// </summary>
        /// <param name="stringBuilder">StringBuilder</param>
        /// <returns>StringBuilder</returns>
        internal static StringBuilder CommitTransaction(this StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine("COMMIT;");
            return stringBuilder;
        }

        /// <summary>
        /// Append line PRAGMA foreign_keys = ON
        /// </summary>
        /// <param name="stringBuilder">StringBuilder</param>
        /// <returns>StringBuilder</returns>
        internal static StringBuilder PragmaForeignKeysOn(this StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine("PRAGMA foreign_keys = ON;");
            return stringBuilder;
        }
    }
}
