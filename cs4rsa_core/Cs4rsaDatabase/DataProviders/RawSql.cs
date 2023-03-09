using Microsoft.EntityFrameworkCore;

using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;

namespace Cs4rsa.Cs4rsaDatabase.DataProviders
{
    public class RawSql
    {
        private readonly DbConnection _connection;
        public RawSql(Cs4rsaDbContext dbContext)
        {
            _connection = dbContext.Database.GetDbConnection();
        }

        /// <summary>
        /// Thực thi SQL Scalar.
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu trả về.</typeparam>
        /// <param name="sql">Câu lệnh SQL.</param>
        /// <returns><typeparamref name="T"/></returns>
        public T ExecScalar<T>(string sql)
        {
            Debug.WriteLine(sql);
            DbCommand cmd = _connection.CreateCommand();
            cmd.CommandText = sql;
            Debug.WriteLine(cmd.CommandText);

            if (_connection.State.Equals(ConnectionState.Closed)) _connection.Open();
            T result = (T)cmd.ExecuteScalar();
            if (_connection.State.Equals(ConnectionState.Open)) _connection.Close();

            cmd.Dispose();
            return result;
        }

        /// <summary>
        /// Thực thi truy vấn SQL.
        /// </summary>
        /// <typeparam name="T">Kết quả mong muốn mà phương thức xử lý sẽ trả về.</typeparam>
        /// <param name="sql">Câu lệnh SQL.</param>
        /// <param name="handleReader">Phương thức tiền xử lý kết quả từ <see cref="DbDataReader"></see></param>
        /// <returns><typeparamref name="T"/></returns>
        public T ExecReader<T>(
            string sql
          , Func<DbDataReader, T> handleReader)
        {
            Debug.Assert(handleReader != null);
            Debug.Assert(sql != null);
            Debug.WriteLine(sql);

            DbCommand cmd = _connection.CreateCommand();
            cmd.CommandText = sql;
            if (_connection.State.Equals(ConnectionState.Closed)) _connection.Open();
            DbDataReader reader = cmd.ExecuteReader();

            T result = handleReader(reader);

            cmd.Dispose();
            reader.Close();
            if (_connection.State.Equals(ConnectionState.Open)) _connection.Close();
            return result;
        }
    }
}
