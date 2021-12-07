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
using System.Data;

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

                int temp = int.Parse((String)ExecuiteScaler(SqlQueries.CheckTabelsExist, new List<MySqlParameter>()));
                
                if (temp != 1)
                {
                    return CreateTables();
                }

                return "Tables already exist !!";
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
                List<MySqlParameter> Z = new List<MySqlParameter>();

                ExecuiteNonQuery(SqlQueries.CreateBanksTable, Z);
                ExecuiteNonQuery(SqlQueries.CreateCustomerAccountsTable, Z);
                ExecuiteNonQuery(SqlQueries.CreateStaffAccountsTable, Z);
                ExecuiteNonQuery(SqlQueries.CreateTransactionsTable, Z);
                ExecuiteNonQuery(SqlQueries.CreateCurrencyTable, Z);

                return "Succesfully created all tables !!";
            }
            catch (Exception e)
            {
                return $"SQL ERROR: {e.ToString()}";
            }
        }

        public DataTable ExecuteReader(string query, List<MySqlParameter> sqlParameters)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                using (MySqlDataAdapter adr = new MySqlDataAdapter())
                {
                    try
                    {
                        adr.SelectCommand = new MySqlCommand(query, conn);
                        adr.SelectCommand.Parameters.AddRange(sqlParameters.ToArray());

                        DataTable dt = new DataTable();
                        adr.Fill(dt);

                        return dt;
                    }
                    catch(Exception e) {
                        return new DataTable();
                    }
                    
                }
            }
        }

        public int ExecuiteNonQuery(string query, List<MySqlParameter> sqlParameters)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddRange(sqlParameters.ToArray());
                    cmd.Connection.Open();
                    var effectedRows = cmd.ExecuteNonQuery();
                    return effectedRows;
                }
            }
        }

        public object ExecuiteScaler(string query, List<MySqlParameter> sqlParameters)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                { 
                    cmd.Parameters.AddRange(sqlParameters.ToArray());

                    cmd.Connection.Open();

                    var value = cmd.ExecuteScalar();
                    return value;
                }
            }
        }

    }
}