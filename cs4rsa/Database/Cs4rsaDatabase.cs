using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;

namespace cs4rsa.Database
{
    /// <summary>
    /// Class này chứa các phương thức thao tác với database
    /// </summary>
    class Cs4rsaDatabase
    {
        private static readonly string databaseName = "cs4rsadb.db";
        public static readonly string ConnectString = $"Data Source={databaseName};Version=3;";

        private static readonly Cs4rsaDatabase instance = new Cs4rsaDatabase();
        private static SQLiteConnection connection;

        private Cs4rsaDatabase()
        {
            connection = new SQLiteConnection(ConnectString);
        }

        public static Cs4rsaDatabase GetInstance()
        {
            return instance;
        }

        private void OpenConnection()
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
        }

        private void CloseConnection()
        {
            if (connection.State != ConnectionState.Closed)
            {
                connection.Close();
            }
        }

        public DataTable GetDataTable(string sqlQueryString)
        {
            OpenConnection();
            DataTable dataTable = new DataTable();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(sqlQueryString, connection);
            adapter.Fill(dataTable);
            CloseConnection();
            return dataTable;
        }

        public T GetScalar<T>(string queryString)
        {
            OpenConnection();
            SQLiteCommand command = new SQLiteCommand(queryString, connection);
            T result = (T)command.ExecuteScalar();
            CloseConnection();
            return result;
        }

        public int DoSomething(string doSQLString)
        {
            OpenConnection();
            SQLiteCommand command = new SQLiteCommand(doSQLString, connection);
            int result = command.ExecuteNonQuery();
            CloseConnection();
            return result;
        }
    }
}
