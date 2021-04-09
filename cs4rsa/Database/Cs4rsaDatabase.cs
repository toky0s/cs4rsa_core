using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;

namespace cs4rsa.Database
{
    class Cs4rsaDatabase
    {
        private string connectString;
        private SQLiteConnection connection;

        public Cs4rsaDatabase(string connectString)
        {
            this.connectString = connectString;
            connection = new SQLiteConnection(connectString);
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

        public long CountSomething(string countQueryString)
        {
            OpenConnection();
            SQLiteCommand command = new SQLiteCommand(countQueryString, connection);
            long result = (long)command.ExecuteScalar();
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
