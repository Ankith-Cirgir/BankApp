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

        public string AddBank(string name, float sRTGS, float sIMPS, float oRTGS, float oIMPS)
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

        public float GetBalance(string accountId)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(String.Format(SqlQueries.GetBalance, accountId), conn))
                    {
                        cmd.Connection.Open();
                        MySqlDataReader reader = cmd.ExecuteReader();
                        string temp = "";
                        while (reader.Read())
                        {
                            temp += reader.GetString(0);
                        }

                        return Convert.ToInt32(temp);

                    }
                }
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        public string GetName(string accountId)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(String.Format(SqlQueries.GetName, accountId), conn))
                    {
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
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        public bool DeleteCustomerAccount(string accountId)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(String.Format(SqlQueries.DeleteCustomerAccount, accountId), conn))
                    {
                        cmd.Connection.Open();
                        MySqlDataReader reader = cmd.ExecuteReader();
                        return true;

                    }
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public string UpdateCustomerPassword(string accountId, string newPassword)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(String.Format(SqlQueries.UpdatePassword, newPassword, accountId), conn))
                    {
                        cmd.Connection.Open();
                        MySqlDataReader reader = cmd.ExecuteReader();
                        return newPassword;

                    }
                }
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        public string UpdateCustomerName(string accountId, string newName)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(String.Format(SqlQueries.UpdateName, newName, accountId), conn))
                    {
                        cmd.Connection.Open();
                        MySqlDataReader reader = cmd.ExecuteReader();
                        return newName;

                    }
                }
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        public bool AuthenticateCustomer(string accountId, string password)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(String.Format(SqlQueries.AuthenticateCustomer, accountId), conn))
                    {
                        cmd.Connection.Open();
                        MySqlDataReader reader = cmd.ExecuteReader();
                        string temp = "";
                        while (reader.Read())
                        {
                            temp += reader.GetString(0);
                        }

                        return temp == "1" ? true : false;
                    }
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool Authenticatestaff(string accountId, string password)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(String.Format(SqlQueries.AuthenticateStaff, accountId), conn))
                    {
                        cmd.Connection.Open();
                        MySqlDataReader reader = cmd.ExecuteReader();
                        string temp = "";
                        while (reader.Read())
                        {
                            temp += reader.GetString(0);
                        }

                        return temp == "1" ? true : false;
                    }
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public float UpdatesRTGS(float val, string bankId)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(String.Format(SqlQueries.UpdatesRTGS, val, bankId), conn))
                    {
                        cmd.Connection.Open();
                        MySqlDataReader reader = cmd.ExecuteReader();
                        return val;

                    }
                }
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        public float UpdatesIMPS(float val, string bankId)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(String.Format(SqlQueries.UpdatesIMPS, val, bankId), conn))
                    {
                        cmd.Connection.Open();
                        MySqlDataReader reader = cmd.ExecuteReader();
                        return val;

                    }
                }
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        public float UpdateoRTGS(float val, string bankId)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(String.Format(SqlQueries.UpdateoRTGS, val, bankId), conn))
                    {
                        cmd.Connection.Open();
                        MySqlDataReader reader = cmd.ExecuteReader();
                        return val;

                    }
                }
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        public float UpdateoIMPS(float val, string bankId)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(String.Format(SqlQueries.UpdateoIMPS, val, bankId), conn))
                    {
                        cmd.Connection.Open();
                        MySqlDataReader reader = cmd.ExecuteReader();
                        return val;

                    }
                }
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        public string GetBankProfits(string bankId)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(String.Format(SqlQueries.GetBankProfits, bankId), conn))
                    {
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
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        public bool AddCurrency(string name, float value, string bankId)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(String.Format(SqlQueries.AddCurrency, name, bankId, value), conn))
                    {
                        cmd.Connection.Open();
                        MySqlDataReader reader = cmd.ExecuteReader();
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public string GetBankId(string accountId)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(String.Format(SqlQueries.GetBankId, accountId), conn))
                    {
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
            catch (Exception e)
            {
                return e.ToString();
            }
        }


        public void GenerateTransaction(string rId, float amount, int type)
        {
            string TId = $"{GetBankId(rId)}{rId}{GetDateTimeNow(true)}";
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(String.Format(SqlQueries.InsertTransaction, TId, amount, type, GetDateTimeNow(false), null,rId), conn))
                    {
                        cmd.Connection.Open();
                        MySqlDataReader reader = cmd.ExecuteReader();
                    }
                }
            }
            catch
            {

            }
        }

        public string DepositAmount(string accountId, float amount, string currencyName)
        {
            try
            {
                float currentBalance = GetBalance(accountId);
                float newBalance = currentBalance + amount;
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(String.Format(SqlQueries.UpdateBalance, newBalance, accountId), conn))
                    {
                        cmd.Connection.Open();
                        MySqlDataReader reader = cmd.ExecuteReader();
                        GenerateTransaction(accountId,amount,2);
                        return GetName(accountId);
                    }
                }
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        public bool WithdrawAmount(string accountId, float amount)
        {
            try
            {
                float newBalance = GetBalance(accountId) - amount;
                if (newBalance < 0)
                {
                    return false;
                }
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(String.Format(SqlQueries.UpdateBalance, newBalance, accountId), conn))
                    {
                        cmd.Connection.Open();
                        MySqlDataReader reader = cmd.ExecuteReader();
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }


    }
}
