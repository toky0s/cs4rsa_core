using Cs4rsa.Constants;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Cs4rsa.Cs4rsaDatabase.DataProviders
{
    public class RawSql
    {
        private readonly SQLiteConnection _connection;
        public RawSql()
        {
            _connection = new SQLiteConnection(VmConstants.DbConnectionString);
        }

        private void OpenConnection()
        {
            if (_connection.State.Equals(ConnectionState.Closed)) _connection.Open();
        }

        private void CloseConnection(bool isNestedOrIEnumerable = false)
        {
            Debug.WriteLineIf(isNestedOrIEnumerable, "CloseConnection: Fail because it is in nested query");
            Debug.WriteLineIf(!isNestedOrIEnumerable, "CloseConnection: Success");
            if (_connection.State.Equals(ConnectionState.Open) && !isNestedOrIEnumerable)
            {
                _connection.Close();
            }
        }

        /// <summary>
        /// Thực thi SQL Scalar.
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu trả về.</typeparam>
        /// <param name="sql">Câu lệnh SQL.</param>
        public T ExecScalar<T>(
            string sql
          , IDictionary<string, object> sqlParams
          , T defaultValueIfNull)
        {
            return ExecScalar(sql, sqlParams, defaultValueIfNull, false);
        }

        /// <summary>
        /// Thực thi SQL Scalar.
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu trả về.</typeparam>
        /// <param name="sql">Câu lệnh SQL.</param>
        public T ExecScalar<T>(
            string sql
          , IDictionary<string, object> sqlParams
          , T defaultValueIfNull
          , bool isNestedOrIEnumerable)
        {
            Debug.Assert(sql != null);
            Debug.Assert(_connection != null);
            Debug.WriteLine(sql);
            using SQLiteCommand cmd = _connection.CreateCommand();
            cmd.CommandText = sql;
            AddParams(cmd, sqlParams);

            OpenConnection();
            object result = cmd.ExecuteScalar();
            CloseConnection(isNestedOrIEnumerable);

            if (result == null) return defaultValueIfNull;
            return (T)result;
        }

        public T ExecReaderGetFirstOrDefault<T>(
            string sql
          , IDictionary<string, object> sqlParams
          , Func<SQLiteDataReader, T> record)
        {
            var result = ExecReader(
                sql
              , sqlParams
              , record).ToList();
            if (result.Count > 0) return result[0];
            return default;
        }

        /// <summary>
        /// Thực thi truy vấn SQL.
        /// </summary>
        /// <remarks>
        /// Không thực hiện các <see cref="ExecReader"/> lồng.
        /// </remarks>
        /// <typeparam name="T">Kết quả mong muốn mà phương thức xử lý sẽ trả về.</typeparam>
        /// <param name="sql">Câu lệnh SQL.</param>
        /// <param name="record">Phương thức tiền xử lý kết quả từ <see cref="IDataRecord"></see></param>
        /// <returns><typeparamref name="T"/></returns>
        public IEnumerable<T> ExecReader<T>(
            string sql
          , IDictionary<string, object> sqlParams
          , Func<SQLiteDataReader, T> record)
        {
            try
            {
                return ExecReader(sql, sqlParams, record, false);
            }
            catch (InvalidOperationException ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("May be you are performing a nested query operation.");
                Debug.WriteLine("In this case, you can use another ExecReader overload with isNestedOrInIEnumerable param is True");
                throw ex;
            }
        }

        /// <summary>
        /// Thực thi truy vấn SQL.
        /// </summary>
        /// <remarks>
        /// Không thực hiện các <see cref="ExecReader"/> lồng.
        /// </remarks>
        /// <typeparam name="T">Kết quả mong muốn mà phương thức xử lý sẽ trả về.</typeparam>
        /// <param name="sql">Câu lệnh SQL.</param>
        /// <param name="record">Phương thức tiền xử lý kết quả từ <see cref="IDataRecord"></see></param>
        /// <param name="isNestedOrInIEnumerable">
        /// Các truy vấn có cấu trúc lồng nhau như trong Foreach,
        /// hoặc kết quả được trả về dưới dạng IEnumerable.
        /// </param>
        /// <returns><typeparamref name="T"/></returns>
        public IEnumerable<T> ExecReader<T>(
            string sql
          , IDictionary<string, object> sqlParams
          , Func<SQLiteDataReader, T> record
          , bool isNestedOrInIEnumerable)
        {
            Debug.Assert(record != null);
            Debug.Assert(sql != null);
            Debug.WriteLine(BeautifyParams(sqlParams));
            Debug.WriteLine(sql);

            using SQLiteCommand cmd = _connection.CreateCommand();
            cmd.CommandText = sql;
            AddParams(cmd, sqlParams);
            OpenConnection();
            using SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.HasRows)
            {
                while (reader.Read())
                {
                    yield return record(reader);
                }
            }
            CloseConnection(isNestedOrInIEnumerable);
        }

        /// <summary>
        /// Thực thi câu lệnh DML.
        /// </summary>
        /// <param name="sql">Câu lệnh SQL.</param>
        /// <param name="sqlParams">Danh sách các tham số.</param>
        /// <returns>Số record được thao tác.</returns>
        public int ExecNonQuery(string sql, IDictionary<string, object> sqlParams)
        {
            Debug.WriteLine(BeautifyParams(sqlParams));
            Debug.WriteLine(sql);
            using SQLiteCommand cmd = _connection.CreateCommand();
            cmd.CommandText = sql;
            AddParams(cmd, sqlParams);

            if (_connection.State.Equals(ConnectionState.Closed)) _connection.Open();
            int result = cmd.ExecuteNonQuery(CommandBehavior.SingleResult);
            if (_connection.State.Equals(ConnectionState.Open)) _connection.Close();
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
                handler.AppendFormatted(
                    paramValue.Value.ToString().Length <= limit
                    ? paramValue.Value
                    : paramValue.Value.ToString()[limit..] + $"...({paramValue.Value.ToString().Length - limit})"
                );
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
            if (sqlParams != null)
            {
                foreach (KeyValuePair<string, object> paramValue in sqlParams)
                {
                    cmd.Parameters.AddWithValue(paramValue.Key, paramValue.Value);
                }
            }
        }

        public static string UseFunction<T>(params string[] args) where T : SQLiteFunction
        {
            StringBuilder sb = new();
            string fn = sb
                .Append(typeof(T).Name)
                .Append('(')
                .Append(string.Join(',', args))
                .Append(')')
                .ToString();
            Debug.WriteLine($"Use function {fn}");
            return fn;
        }
    }

    /// <summary>
    /// Hàm thay thế ký tự tiếng Việt có dấu thành không dấu.
    /// </summary>
    /// 
    [SQLiteFunction(Name = "FuncRemoveAccent", Arguments = 1, FuncType = FunctionType.Scalar, FuncFlags = SQLiteFunctionFlags.SQLITE_DETERMINISTIC)]
    public class FuncRemoveAccent : SQLiteFunction
    {
        public override object Invoke(object[] args)
        {
            string s = (string)args[0];
            Regex regex = new("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, string.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }
    }
}
