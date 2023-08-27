using Cs4rsa.Constants;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Cs4rsa.Cs4rsaDatabase.DataProviders
{
    public abstract class RawSql
    {
        public static void CreateDbIfNotExist()
        {
            SQLiteConnection.CreateFile(VmConstants.DbFilePath);
            string text = File.ReadAllText(VmConstants.InitDbFilePath);
            ExecNonQuery(text);
        }

        /// <summary>
        /// Thực thi SQL Scalar.
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu trả về.</typeparam>
        /// <param name="sql">Câu lệnh SQL.</param>
        /// <param name="defaultValueIfNull">Giá trị trả về mặc định nếu kết quả NULL.</param>
        public static T ExecScalar<T>(
            string sql
          , T defaultValueIfNull)
        {
            return ExecScalar(sql, null, defaultValueIfNull);
        }

        /// <summary>
        /// Thực thi SQL Scalar.
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu trả về.</typeparam>
        /// <param name="sql">Câu lệnh SQL.</param>
        /// <param name="sqlParams">Tham số.</param>
        /// <param name="defaultValueIfNull">Giá trị trả về mặc định nếu kết quả NULL.</param>
        public static T ExecScalar<T>(
            string sql
          , IDictionary<string, object> sqlParams
          , T defaultValueIfNull)
        {
            Debug.WriteLine(BeautifyParams(sqlParams));
            Debug.WriteLine(sql);
            using SQLiteConnection connection = new(VmConstants.DbConnectionString);
            connection.Open();
            using SQLiteCommand cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            AddParams(cmd, sqlParams);
            object result = cmd.ExecuteScalar();
            if (result == null) return defaultValueIfNull;
            return (T)result;
        }

        public static T ExecReaderGetFirstOrDefault<T>(
            string sql
          , IDictionary<string, object> sqlParams
          , Func<SQLiteDataReader, T> record)
        {
            List<T> result = ExecReader(sql, sqlParams, record);
            return result.Count > 0 ? result[0] : default;
        }

        /// <summary>
        /// Thực thi truy vấn SQL.
        /// </summary>
        /// <typeparam name="T">Kết quả mong muốn mà phương thức xử lý sẽ trả về.</typeparam>
        /// <param name="sql">Câu lệnh SQL.</param>
        /// <param name="record">Phương thức tiền xử lý kết quả từ <see cref="IDataRecord"></see></param>
        /// <returns><typeparamref name="T"/></returns>
        public static List<T> ExecReader<T>(string sql, Func<SQLiteDataReader, T> record)
        {
            return ExecReader(sql, null, record);
        }

        /// <summary>
        /// Thực thi truy vấn SQL.
        /// </summary>
        /// <typeparam name="T">Kết quả mong muốn mà phương thức xử lý sẽ trả về.</typeparam>
        /// <param name="sql">Câu lệnh SQL.</param>
        /// <param name="sqlParams">Params</param>
        /// <param name="record">Phương thức tiền xử lý kết quả từ <see cref="IDataRecord"></see></param>
        /// <returns>IEnumerable<typeparamref name="T"/></returns>
        public static List<T> ExecReader<T>(
            string sql
          , IDictionary<string, object> sqlParams
          , Func<SQLiteDataReader, T> record)
        {
            Debug.WriteLine(BeautifyParams(sqlParams));
            Debug.WriteLine(sql);

            using SQLiteConnection connection = new(VmConstants.DbConnectionString);
            connection.Open();
            using SQLiteCommand cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            AddParams(cmd, sqlParams);
            using SQLiteDataReader reader = cmd.ExecuteReader();
            List<T> result = new();
            while (reader.HasRows)
            {
                while (reader.Read())
                {
                    result.Add(record(reader));
                }
            }
            return result;
        }

        /// <summary>
        /// Thực thi câu lệnh DML.
        /// </summary>
        /// <param name="sql">Câu lệnh SQL mà không cần tham số.</param>
        /// <returns>Số record được thao tác.</returns>
        public static int ExecNonQuery(string sql)
        {
            return ExecNonQuery(sql, null);
        }

        /// <summary>
        /// Thực thi câu lệnh DML.
        /// </summary>
        /// <param name="sql">Câu lệnh SQL.</param>
        /// <param name="sqlParams">Danh sách các tham số.</param>
        /// <returns>Số record được thao tác.</returns>
        public static int ExecNonQuery(string sql, IDictionary<string, object> sqlParams)
        {
            Debug.WriteLine(BeautifyParams(sqlParams));
            Debug.WriteLine(sql);
            using SQLiteConnection connection = new(VmConstants.DbConnectionString);
            connection.Open();
            using SQLiteCommand cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            AddParams(cmd, sqlParams);
            int result = cmd.ExecuteNonQuery();
            return result;
        }

        private static string BeautifyParams(IDictionary<string, object> sqlParams)
        {
            if (sqlParams == null) return "Command was executed without param.";
            StringBuilder sb = new();
            StringBuilder.AppendInterpolatedStringHandler handler = new(literalLength: 23, formattedCount: 1, sb);
            handler.AppendLiteral("====== Params (");
            handler.AppendFormatted(sqlParams.Count);
            handler.AppendLiteral(") ======");
            sb.AppendLine(ref handler);
            StringBuilder.AppendInterpolatedStringHandler handler2 = new(literalLength: 2, formattedCount: 2, sb);
            const int limit = 30;
            foreach (KeyValuePair<string, object> paramValue in sqlParams)
            {
                handler.AppendFormatted(paramValue.Key);
                handler.AppendLiteral(": ");

                if (paramValue.Value != null)
                {
                    handler.AppendFormatted(
                        paramValue.Value.ToString()!.Length <= limit
                        ? paramValue.Value
                        : paramValue.Value.ToString()?[limit..] + $"...({paramValue.Value.ToString()!.Length - limit})"
                    );
                }
                else
                {
                    handler.AppendFormatted("NULL");
                }

                sb.AppendLine(ref handler2);
            }
            handler.AppendLiteral("====== Params (");
            handler.AppendFormatted(sqlParams.Count);
            handler.AppendLiteral(") ======");
            sb.AppendLine(ref handler);
            return sb.ToString();
        }

        private static void AddParams(SQLiteCommand cmd, IDictionary<string, object> sqlParams)
        {
            if (sqlParams == null) return;
            foreach (KeyValuePair<string, object> paramValue in sqlParams)
            {
                cmd.Parameters.AddWithValue(paramValue.Key, paramValue.Value);
            }
        }
    }

    #region String Builder Extension
    public static class RawSqlStringBuilderExtension
    {
        private static readonly Dictionary<string, EnumerableRowCollection<string>> _tableColumnNames = new();

        /// <summary>
        /// Hỗ trợ tạo tự động mệnh đề Select
        /// với tên các cột của bảng được truyền vào.
        /// </summary>
        /// <typeparam name="T">
        /// Đối tượng DAO matching với 
        /// tên bảng trong cơ sở dữ liệu vật lý.
        /// </typeparam>
        /// <param name="sb">StringBuilder</param>
        public static StringBuilder AppendSelectColumns<T>(this StringBuilder sb)
        {
            string tableName = typeof(T).Name + "s";
            if (_tableColumnNames.TryGetValue(tableName, out EnumerableRowCollection<string> value))
            {
                sb.AppendLine(string.Join("\n, ", value));
            }
            else
            {
                using SQLiteConnection cnn = new(VmConstants.DbConnectionString);
                cnn.Open();
                DataTable columnTable = cnn.GetSchema("Columns");
                EnumerableRowCollection<string> rowCollection =
                    from c in columnTable.AsEnumerable()
                    where c["TABLE_NAME"].Equals(typeof(T).Name + "s")
                    select c["COLUMN_NAME"].ToString();
                _tableColumnNames.Add(tableName, rowCollection);
                sb.AppendLine(string.Join("\n, ", rowCollection));
            }
            return sb;
        }

        /// <summary>
        /// Loại bỏ ký tự cuối cùng trong StringBuilder.
        /// </summary>
        /// <remarks>
        /// Được sử dụng trong trường hợp Bulk Insert, để loại bỏ
        /// dấu phẩy ở cuối câu lệnh INSERT.
        /// </remarks>
        /// <param name="sb">StringBuilder</param>
        /// <returns>StringBuilder</returns>
        public static void RemoveLastCharAfterAppendLine(this StringBuilder sb)
        {
            sb.Length -= 3;
        }
    }
    #endregion

    #region Utils Function
    public abstract class MathUtils
    {
        public static int CountPage(int amount, int limit)
        {
            int page = amount / limit;
            if (page * limit < amount)
            {
                return page + 1;
            }
            return page;
        }
    }
    #endregion
}
