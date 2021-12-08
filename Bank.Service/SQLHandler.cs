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
        private string ConnectionSting;

        public SQLHandler()
        {
            Init();
        }

        private bool Init()
        {
            ConnectionSting = "server=localhost;user=root;database=bankapp;port=3306;password=admin";
            try
            {

                int tableCount = int.Parse((String)ExecuiteScaler(SqlQueries.CheckTabelsExist));
                
                if (tableCount == 0)
                {
                    return CreateTables();
                }

                return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool CreateTables()
        {
            try
            {
                ExecuiteNonQuery(SqlQueries.CreateBanksTable);
                ExecuiteNonQuery(SqlQueries.CreateCustomerAccountsTable);
                ExecuiteNonQuery(SqlQueries.CreateStaffAccountsTable);
                ExecuiteNonQuery(SqlQueries.CreateTransactionsTable);
                ExecuiteNonQuery(SqlQueries.CreateCurrencyTable);

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public DataTable ExecuteReader(string query, List<MySqlParameter> sqlParameters)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnectionSting))
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

        public int ExecuiteNonQuery(string query, List<MySqlParameter> sqlParameters = null)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnectionSting))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if(sqlParameters != null)
                    {
                        cmd.Parameters.AddRange(sqlParameters.ToArray());
                    }
                    cmd.Connection.Open();

                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public object ExecuiteScaler(string query, List<MySqlParameter> sqlParameters = null)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnectionSting))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if(sqlParameters == null)
                    {
                        cmd.Parameters.AddRange(sqlParameters.ToArray());
                    }

                    cmd.Connection.Open();
                    return cmd.ExecuteScalar();
                }
            }
        }

    }
}