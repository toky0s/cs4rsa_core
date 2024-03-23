using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;

namespace Cs4rsa.Database.DataProviders
{
    public class RawSql
    {
        /// <summary>
        /// Connection string
        /// </summary>
        public string CnnStr { get; }
        public RawSql(string cnnStr)
        {
            CnnStr = cnnStr;
        }

        public void CreateDbIfNotExist(string dbFilePath, string sqlInitDbFilePath)
        {
            SQLiteConnection.CreateFile(dbFilePath);
            var text = File.ReadAllText(sqlInitDbFilePath);
            ExecNonQuery(text);
        }

        /// <summary>
        /// Thực thi SQL Scalar.
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu trả về.</typeparam>
        /// <param name="sql">Câu lệnh SQL.</param>
        /// <param name="defaultValueIfNull">Giá trị trả về mặc định nếu kết quả NULL.</param>
        /// <param name="cnnStr">Connection string.</param>
        public T ExecScalar<T>(
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
        /// <param name="cnnStr">Connection string.</param>
        public T ExecScalar<T>(
            string sql
          , IDictionary<string, object> sqlParams
          , T defaultValueIfNull)
        {
            using (var connection = new SQLiteConnection(CnnStr))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = sql;
                    AddParams(cmd, sqlParams);
                    var result = cmd.ExecuteScalar();
                    if (result == null) return defaultValueIfNull;
                    return (T)Convert.ChangeType(result, typeof(T));
                    //return (T)result;
                }
            }
        }

        public T ExecReaderGetFirstOrDefault<T>(
            string sql
          , IDictionary<string, object> sqlParams
          , Func<SQLiteDataReader, T> record)
        {
            var result = ExecReader(sql, sqlParams, record);
            return result.Count > 0 ? result[0] : default;
        }

        /// <summary>
        /// Thực thi truy vấn SQL.
        /// </summary>
        /// <typeparam name="T">Kết quả mong muốn mà phương thức xử lý sẽ trả về.</typeparam>
        /// <param name="sql">Câu lệnh SQL.</param>
        /// <param name="record">Phương thức tiền xử lý kết quả từ <see cref="IDataRecord"></see></param>
        /// <param name="cnnStr">Connection string.</param>
        /// <returns><typeparamref name="T"/></returns>
        public List<T> ExecReader<T>(string sql, Func<SQLiteDataReader, T> record)
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
        /// <param name="cnnStr">Connection string.</param>
        /// <returns>IEnumerable <typeparamref name="T"/></returns>
        public List<T> ExecReader<T>(
            string sql
          , IDictionary<string, object> sqlParams
          , Func<SQLiteDataReader, T> record)
        {
            using (var connection = new SQLiteConnection(CnnStr))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = sql;
                    AddParams(cmd, sqlParams);
                    using (var reader = cmd.ExecuteReader())
                    {
                        var result = new List<T>();
                        while (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                result.Add(record(reader));
                            }
                        }
                        return result;
                    }
                }
            }
        }

        public void ExecReader(string sql, IDictionary<string, object> sqlParams, Action<SQLiteDataReader> record)
        {
            using (var connection = new SQLiteConnection(CnnStr))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = sql;
                    AddParams(cmd, sqlParams);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                record(reader);
                            }
                        }
                    }
                }
            }
        }

        public void ExecReader(string sql, Action<SQLiteDataReader> record)
        {
            using (var connection = new SQLiteConnection(CnnStr))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = sql;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                record(reader);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Thực thi câu lệnh DML.
        /// </summary>
        /// <param name="sql">Câu lệnh SQL mà không cần tham số.</param>
        /// <param name="cnnStr">Connection string.</param>
        /// <returns>Số record được thao tác.</returns>
        public int ExecNonQuery(string sql)
        {
            return ExecNonQuery(sql, null);
        }

        /// <summary>
        /// Thực thi câu lệnh DML.
        /// </summary>
        /// <param name="sql">Câu lệnh SQL.</param>
        /// <param name="sqlParams">Danh sách các tham số.</param>
        /// <param name="cnnStr">Connection string.</param>
        /// <returns>Số record được thao tác.</returns>
        public int ExecNonQuery(string sql, IDictionary<string, object> sqlParams)
        {
            using (var connection = new SQLiteConnection(CnnStr))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = sql;
                    AddParams(cmd, sqlParams);
                    var result = cmd.ExecuteNonQuery();
                    return result;
                }
            }
        }

        private string BeautifyParams(IDictionary<string, object> sqlParams)
        {
            if (sqlParams == null || sqlParams.Count == 0)
            {
                return "Command was executed without param.";
            }
            var sb = new StringBuilder();
            sb.Append("====== Params (");
            sb.Append(sqlParams.Count);
            sb.AppendLine(") ======");
            const int limit = 30;
            foreach (var paramValue in sqlParams)
            {
                sb.Append(paramValue.Key);
                sb.Append(": ");

                if (paramValue.Value is null)
                {
                    sb.Append("NULL");
                }
                else
                {
                    sb.AppendLine(
                        paramValue.Value.ToString().Length <= limit
                        ? paramValue.Value.ToString()
                        : paramValue.Value.ToString().Substring(0, limit) + $"...({paramValue.Value.ToString().Length - limit})"
                    );
                }
            }
            sb.Append("====== Params (");
            sb.Append(sqlParams.Count);
            sb.AppendLine(") ======");
            return sb.ToString();
        }

        private void AddParams(SQLiteCommand cmd, IDictionary<string, object> sqlParams)
        {
            if (sqlParams == null) return;
            foreach (var paramValue in sqlParams)
            {
                cmd.Parameters.AddWithValue(paramValue.Key, paramValue.Value);
            }
        }
    }

    #region String Builder Extension
    public static class RawSqlStringBuilderExtension
    {
        private static readonly Dictionary<string, IEnumerable<string>> TableColumnNames = new Dictionary<string, IEnumerable<string>>();

        /// <summary>
        /// Hỗ trợ tạo tự động mệnh đề Select
        /// với tên các cột của bảng được truyền vào.
        /// </summary>
        /// <typeparam name="T">
        /// Đối tượng DAO matching với 
        /// tên bảng trong cơ sở dữ liệu vật lý.
        /// </typeparam>
        /// <param name="sb">StringBuilder</param>
        public static StringBuilder AppendSelectColumns<T>(this StringBuilder sb, string cnnStr)
        {
            var tableName = typeof(T).Name + "s";
            if (TableColumnNames.TryGetValue(tableName, out var value))
            {
                sb.AppendLine(string.Join("\n, ", value));
            }
            else
            {
                using (var cnn = new SQLiteConnection(cnnStr))
                {
                    cnn.Open();
                    var columnTable = cnn.GetSchema("Columns");

                    var columnNames = (
                        from DataRow dataRow in columnTable.Rows
                        where dataRow["TABLE_NAME"].Equals(typeof(T).Name + "s")
                        select dataRow["COLUMN_NAME"].ToString()
                    ).ToList();
                    TableColumnNames.Add(tableName, columnNames);
                    sb.AppendLine(string.Join("\n, ", columnNames));
                }
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
        public static StringBuilder RemoveLastCharAfterAppendLine(this StringBuilder sb)
        {
            sb.Length -= 3;
            return sb;
        }
    }
    #endregion

    #region Utils Function
    public abstract class MathUtils
    {
        public static int CountPage(int amount, int limit)
        {
            var page = amount / limit;
            if (page * limit < amount)
            {
                return page + 1;
            }
            return page;
        }
    }
    #endregion
}
