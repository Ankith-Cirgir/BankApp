using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace BankApp.Service
{
    class SQLHandler
    {
        private string connStr;

        public string GetDateTimeNow(bool forId)
        {
            return forId ? DateTime.Now.ToString("ddMMyyyyHHmmss") : DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }
        public string init()
        {
            connStr = "server=localhost;user=root;database=bankapp;port=3306;password=admin";
            try
            {
                
                CreateStaffAccount("admin", "admin", "admin", "Mon09112021");
                
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
                        if(temp != "1" || temp == null )
                        {
                            return CreateTables();
                        }
                        return "Tables already exist !!";
                    }
                }
            }
            catch(Exception e)
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
                        MySqlDataReader reader = cmd.ExecuteReader();
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

        public string CreateCustomerAccount(string name, string password, string bankId)
        {
            string accountId = $"{name.Substring(0, 3)}{DateTime.Now.ToString("ddMMyyyyHHmmss")}";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(String.Format(SqlQueries.InsertIntoCustomersTable, accountId, bankId, 0, name, password), conn))
                    {
                        cmd.Connection.Open();
                        MySqlDataReader reader = cmd.ExecuteReader();
                    }
                }
            }
            catch (Exception e)
            {
                return e.ToString();
            }

            return accountId;
        }

        public string CreateStaffAccount(string accountId, string name, string password, string bankId)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(String.Format(SqlQueries.InsertIntoStaffsTable, accountId, bankId, name, password), conn))
                    {
                        cmd.Connection.Open();
                        MySqlDataReader reader = cmd.ExecuteReader();
                    }
                }
            }
            catch (Exception e)
            {
                return e.ToString();
            }

            return accountId;
        }

        public string CreateStaffAccount(string name, string password, string bankId)
        {
            string accountId = $"{name.Substring(0, 3)}{GetDateTimeNow(true)}";
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(String.Format(SqlQueries.InsertIntoStaffsTable, accountId, bankId, name, password), conn))
                    {
                        cmd.Connection.Open();
                        MySqlDataReader reader = cmd.ExecuteReader();
                    }
                }
            }
            catch (Exception e)
            {
                return e.ToString();
            }

            return accountId;
        }

        public string AddBank(string name)
        {
            string bankId = $"{name.Substring(0, 3)}{GetDateTimeNow(true)}";
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(String.Format(SqlQueries.InsertIntoBanksTable, bankId, name, 0, 5, 2, 6), conn))
                    {
                        cmd.Connection.Open();
                        MySqlDataReader reader = cmd.ExecuteReader();
                    }
                }
            }
            catch (Exception e)
            {
                return e.ToString();
            }
            return bankId;
        }

        public string AddBank(string name, float sRTGS, float sIMPS, float oRTGS, float oIMPS )
        {
            string bankId = $"{name.Substring(0, 3)}{GetDateTimeNow(true)}";
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(String.Format(SqlQueries.InsertIntoBanksTable, bankId, name, sRTGS, sIMPS, oRTGS, oIMPS), conn))
                    {
                        cmd.Connection.Open();
                        MySqlDataReader reader = cmd.ExecuteReader();
                    }
                }
            }
            catch (Exception e)
            {
                return e.ToString();
            }
            return bankId;
        }

        

    }
}
