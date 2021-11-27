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
            catch 
            {
                return -1;
            }
        }

        public bool UpdateBalance(string accountId, float newBalance)
        {
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
            catch 
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
            catch 
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
            catch 
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
            catch 
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
            catch 
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
            catch 
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
            catch 
            {
                return -1;
            }
        }

        public float GetBankProfits(string bankId)
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

                    return float.Parse(temp);
                }
            }
            
        }

        public void UpdateBankProfits(string bankId, float amount)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(String.Format(SqlQueries.UpdateBankProfits, amount, bankId), conn))
                {
                    cmd.Connection.Open();
                    MySqlDataReader reader = cmd.ExecuteReader();
                }
            }
        }

        public float GetBanksRTGSCharges(string bankId)
        {

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(String.Format(SqlQueries.GetBanksRTGSCharges, bankId), conn))
                {
                    cmd.Connection.Open();
                    MySqlDataReader reader = cmd.ExecuteReader();
                    string temp = "";
                    while (reader.Read())
                    {
                        temp += reader.GetString(0);
                    }

                    return float.Parse(temp);
                }
            }
        }

        public float GetBanksIMPSCharges(string bankId)
        {

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(String.Format(SqlQueries.GetBanksIMPSCharges, bankId), conn))
                {
                    cmd.Connection.Open();
                    MySqlDataReader reader = cmd.ExecuteReader();
                    string temp = "";
                    while (reader.Read())
                    {
                        temp += reader.GetString(0);
                    }

                    return float.Parse(temp);
                }
            }
        }

        public float GetBankoRTGSCharges(string bankId)
        {

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(String.Format(SqlQueries.GetBankoRTGSCharges, bankId), conn))
                {
                    cmd.Connection.Open();
                    MySqlDataReader reader = cmd.ExecuteReader();
                    string temp = "";
                    while (reader.Read())
                    {
                        temp += reader.GetString(0);
                    }

                    return float.Parse(temp);
                }
            }
        }

        public float GetBankoIMPSCharges(string bankId)
        {

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(String.Format(SqlQueries.GetBankoIMPSCharges, bankId), conn))
                {
                    cmd.Connection.Open();
                    MySqlDataReader reader = cmd.ExecuteReader();
                    string temp = "";
                    while (reader.Read())
                    {
                        temp += reader.GetString(0);
                    }

                    return float.Parse(temp);
                }
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
            catch 
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

        public void GenerateTransaction(string senderId, string receiverId, float amount, int type)
        {
            string TId = $"{GetBankId(receiverId)}{receiverId}{GetDateTimeNow(true)}";

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(String.Format(SqlQueries.InsertTransaction, TId, amount, type, GetDateTimeNow(false), senderId, receiverId), conn))
                {
                    cmd.Connection.Open();
                    MySqlDataReader reader = cmd.ExecuteReader();
                }
            }
        }

        public void GenerateTransaction(string receiverId, float amount, int type)
        {
            string TId = $"{GetBankId(receiverId)}{receiverId}{GetDateTimeNow(true)}";

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(String.Format(SqlQueries.InsertTransactionReceiver, TId, amount, type, GetDateTimeNow(false), receiverId), conn))
                {
                    cmd.Connection.Open();
                    MySqlDataReader reader = cmd.ExecuteReader();
                }
            }
        }

        public float GetCurrencyValue(string currencyName)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(String.Format(SqlQueries.GetCurrencyValue, currencyName), conn))
                {
                    cmd.Connection.Open();
                    MySqlDataReader reader = cmd.ExecuteReader();
                    string temp = "";
                    while (reader.Read())
                    {
                        temp += reader.GetValue(0);
                    }
                    return float.Parse(temp, CultureInfo.InvariantCulture.NumberFormat);
                }
            }
        }

        public string DepositAmount(string accountId, float amount, string currencyName)
        {
            try
            {
                float currentBalance = GetBalance(accountId);
                float newBalance = currentBalance + amount * GetCurrencyValue(currencyName);
                
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(String.Format(SqlQueries.UpdateBalance, newBalance, accountId), conn))
                    {
                        cmd.Connection.Open();
                        MySqlDataReader reader = cmd.ExecuteReader();
                        GenerateTransaction(accountId, amount, 2);
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
                        GenerateTransaction(accountId, amount, 1);
                        return true;
                    }
                }
            }
            catch 
            {
                return false;
            }
        }

        public ConsoleTable GetTransactions(string accountId)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(String.Format(SqlQueries.GetTransactions, accountId, accountId), conn))
                    {
                        cmd.Connection.Open();
                        MySqlDataReader reader = cmd.ExecuteReader();
                        ConsoleTable table = new ConsoleTable(new ConsoleTableOptions { Columns = new[] { "TransactionId", "SendersAccountId", "RecieversAccountId", "Type", "Amount", "Time" }, EnableCount = false });
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

        public bool TransferAmountRTGS(string fromId, string toId, float amount)
        {
            float UpdatedAmount;
            string fromBankId = GetBankId(fromId);

            if (GetBalance(fromId) - amount < 0)
            {
                return false;
            }
            if (fromBankId == GetBankId(toId))
            {
                float temp = amount * (GetBanksRTGSCharges(fromBankId) / 100);
                UpdatedAmount = amount - temp;
            }
            else
            {
                float temp = amount * (GetBankoRTGSCharges(fromBankId) / 100);
                UpdatedAmount = amount - temp;
            }

            GenerateTransaction(fromId, toId, amount, 3);

            UpdateBalance(fromId, GetBalance(fromId) - amount);
            UpdateBalance(toId, GetBalance(toId) + UpdatedAmount);

            UpdateBankProfits(fromBankId, GetBankProfits(fromBankId) + (amount - UpdatedAmount));
            
         return true;
        }

        public bool TransferAmountIMPS(string fromId, string toId, float amount)
        {
            float UpdatedAmount;
            string fromBankId = GetBankId(fromId);

            if (GetBalance(fromId) - amount < 0)
            {
                return false;
            }
            if (fromBankId == GetBankId(toId))
            {
                float temp = amount * (GetBanksIMPSCharges(fromBankId) / 100);
                UpdatedAmount = amount - temp;
            }
            else
            {
                float temp = amount * (GetBankoIMPSCharges(fromBankId) / 100);
                UpdatedAmount = amount - temp;
            }

            GenerateTransaction(fromId, toId, amount, 3);

            UpdateBalance(fromId, GetBalance(fromId) - amount);
            UpdateBalance(toId, GetBalance(toId) + UpdatedAmount);

            UpdateBankProfits(fromBankId, GetBankProfits(fromBankId) + (amount - UpdatedAmount));

            return true;
        }

        public float GetTransactionAmount(string transcationsId)
        {

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(String.Format(SqlQueries.GetTransactionAmount, transcationsId), conn))
                {
                    cmd.Connection.Open();
                    MySqlDataReader reader = cmd.ExecuteReader();
                    string temp = "";
                    while (reader.Read())
                    {
                        temp += reader.GetString(0);
                    }

                    return float.Parse(temp);
                }
            }
        }

        public int GetTransactionType(string transcationsId)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(String.Format(SqlQueries.GetTransactionType, transcationsId), conn))
                {
                    cmd.Connection.Open();
                    MySqlDataReader reader = cmd.ExecuteReader();
                    string temp = "";
                    while (reader.Read())
                    {
                        temp += reader.GetString(0);
                    }

                    return int.Parse(temp);
                }
            }
        }

        public string GetTransactionSenderId(string transactionId)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(String.Format(SqlQueries.GetTransactionSenderId, transactionId), conn))
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

        public string GetTransactionReceiverId(string transactionId)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(String.Format(SqlQueries.GetTransactionReceiverId, transactionId), conn))
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

        public bool RevertTransaction(string transactionId)
        {
            try
            {
                float transactionAmount = GetTransactionAmount(transactionId);
                string receiversId = GetTransactionReceiverId(transactionId);
                float receiversBalance = GetBalance(receiversId);

                switch (GetTransactionType(transactionId))
                {
                    case (int)TransactionType.Deposit:
                        if(receiversBalance - transactionAmount >= 0)
                        {
                            float newBalance = receiversBalance - transactionAmount;
                            UpdateBalance(receiversId, newBalance);
                        }
                        else 
                        { 
                            return false;
                        }
                        break;

                    case (int)TransactionType.Withdraw:
                        UpdateBalance(receiversId, receiversBalance + transactionAmount);
                        break;

                    case (int)TransactionType.Transfer:
                        string sendersId = GetTransactionSenderId(transactionId);
                        
                        float newBal = receiversBalance - transactionAmount;
                        UpdateBalance(receiversId, newBal);

                        float sendersBalance = GetBalance(sendersId);
                        UpdateBalance(sendersId, sendersBalance + transactionAmount);
                        break;
                }
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(String.Format(SqlQueries.DeleteTransaction, transactionId), conn))
                    {
                        cmd.Connection.Open();
                        MySqlDataReader reader = cmd.ExecuteReader();
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
    }
}