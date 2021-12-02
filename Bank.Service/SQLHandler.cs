using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Globalization;
using ConsoleTables;
using static BankApp.Model.Transaction;

namespace BankApp.Service
{
    class SQLHandler
    {
        private string connStr;

        public string init()
        {
            connStr = "server=localhost;user=root;database=bankapp;port=3306;password=admin";
            try
            {

                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(SqlQueries.CheckTabelsExist, conn))
                    {
                        cmd.Connection.Open();
                        MySqlDataReader reader = cmd.ExecuteReader();
                        string temp = "";
                        while (reader.Read())
                        {
                            temp += reader.GetString(0);
                        }
                        if (temp != "1" || temp == null)
                        {
                            return CreateTables();
                        }
                        return "Tables already exist !!";
                    }
                }
            }
            catch (Exception e)
            {
                return $"SQL ERROR WHILE INITILIZING DB \n{e}";
            }
        }

        public string CreateTables()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(SqlQueries.CreateBanksTable, conn))
                    {
                        cmd.Connection.Open();
                        MySqlDataReader reader = cmd.ExecuteReader(); // execuite non query // exceute scalar //  DataSet // SQL parameters
                    }
                }

                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(SqlQueries.CreateCustomerAccountsTable, conn))
                    {
                        cmd.Connection.Open();
                        MySqlDataReader reader = cmd.ExecuteReader();
                    }

                }
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(SqlQueries.CreateStaffAccountsTable, conn))
                    {
                        cmd.Connection.Open();
                        MySqlDataReader reader = cmd.ExecuteReader();
                    }

                }
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(SqlQueries.CreateTransactionsTable, conn))
                    {
                        cmd.Connection.Open();
                        MySqlDataReader reader = cmd.ExecuteReader();
                    }

                }

                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(SqlQueries.CreateCurrencyTable, conn))
                    {
                        cmd.Connection.Open();
                        MySqlDataReader reader = cmd.ExecuteReader();
                    }

                }

                return "Succesfully created all tables !!";
            }
            catch (Exception e)
            {
                return "SQL ERROR: " + e.ToString();
            }
        }

        public string GetDateTimeNow(bool forId)
        {
            return forId ? DateTime.Now.ToString("ddMMyyyyHHmmss") : DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        public string ExecuiteReader(string query, List<MySqlParameter> collection)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddRange(collection.ToArray());
                    cmd.Connection.Open();
                    MySqlDataReader reader = cmd.ExecuteReader();
                    string temp = "";
                    while (reader.Read())
                    {
                        temp += reader.GetString(0);
                    }
                    return temp;
                }
            }
        }

        public ConsoleTable ExecuteReader(string query, string[] columns, bool enableCount)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Connection.Open();
                        MySqlDataReader reader = cmd.ExecuteReader();
                        ConsoleTable table = new ConsoleTable(new ConsoleTableOptions { Columns = columns, EnableCount = enableCount });
                        while (reader.Read())
                        {
                            string type = "";
                            switch (reader.GetValue(2))
                            {
                                case (int)TransactionType.Deposit:
                                    type = "Deposit";
                                    break;
                                case (int)TransactionType.Transfer:
                                    type = "Transfer";
                                    break;
                                case (int)TransactionType.Withdraw:
                                    type = "Withdraw";
                                    break;
                            }
                            table.AddRow(reader.GetValue(0), reader.GetValue(4), reader.GetValue(5), type, reader.GetValue(1), reader.GetValue(3));
                        }
                        return table;
                    }
                }
            }
            catch
            {
                return new ConsoleTable();
            }
        }

        public int ExecuiteNonQuery(string query)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Connection.Open();
                    int effectedRows = cmd.ExecuteNonQuery();
                    return effectedRows;
                }
            }
        }

        public int ExecuiteNonQuery(string query, List<MySqlParameter> collection)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddRange(collection.ToArray());
                    cmd.Connection.Open();
                    var effectedRows = cmd.ExecuteNonQuery();
                    return effectedRows;
                }
            }
        }

        public object ExecuiteScaler(string query)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Connection.Open();
                    var value = cmd.ExecuteScalar();
                    return value;
                }
            }
        }

        public object ExecuiteScaler(string query, List<MySqlParameter> collection)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddRange(collection.ToArray());
                    cmd.Connection.Open();
                    var value = cmd.ExecuteScalar();
                    return value;
                }
            }
        }

    }
}