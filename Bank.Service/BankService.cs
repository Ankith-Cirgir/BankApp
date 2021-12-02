using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankApp.Model;
using ConsoleTables;
using MySql.Data.MySqlClient;

namespace BankApp.Service
{
    public class BankService
    {
        private SQLHandler sqlHandler;

        public void init()
        {
            sqlHandler = new SQLHandler();
            sqlHandler.init();

            CreateStaffAccount("admin", "admin", "Mon09112021");
            AddCurrency("INR", 1, "Mon09112021");
        }

        public string GetDateTimeNow(bool forId)
        {
            return forId ? DateTime.Now.ToString("ddMMyyyyHHmmss") : DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        public string AddBank(string name, float sRTGS, float sIMPS, float oRTGS, float oIMPS)
        {
            string bankId = $"{name.Substring(0, 3)}{GetDateTimeNow(true)}";
            sqlHandler.ExecuiteNonQuery(String.Format(SqlQueries.InsertIntoBanksTable, bankId, name, sRTGS, sIMPS, oRTGS, oIMPS));
            return bankId;
        }

        public string AddBank(string name)
        {
            string bankId = $"{name.Substring(0, 3)}{GetDateTimeNow(true)}";
            sqlHandler.ExecuiteNonQuery(String.Format(SqlQueries.InsertIntoBanksTable, bankId, name, 0, 5, 2, 6));

            return bankId;
        }

        public string CreateCustomerAccount(string name, string pass, string bankId)
        {
            string accountId = $"{name.Substring(0, 3)}{GetDateTimeNow(true)}";

            sqlHandler.ExecuiteNonQuery(String.Format(SqlQueries.InsertIntoCustomersTable, accountId, bankId, 0, name, pass));
            return accountId;
        }

        public string CreateStaffAccount(string name, string pass, string bankId)
        {
            string accountId = $"{name.Substring(0, 3)}{GetDateTimeNow(true)}";

            sqlHandler.ExecuiteNonQuery(String.Format(SqlQueries.InsertIntoStaffsTable, accountId, bankId, name, pass));
            return accountId;
        }

        public bool UpdateBalance(string accountId, float newBalance)
        {
            sqlHandler.ExecuiteNonQuery(String.Format(String.Format(SqlQueries.UpdateBalance, newBalance, accountId)));
            return true;
        }

        public void GenerateTransaction(string receiverId, float amount, int type)
        {
            string TId = $"{GetBankId(receiverId)}{receiverId}{GetDateTimeNow(true)}";
            sqlHandler.ExecuiteNonQuery(String.Format(SqlQueries.InsertTransactionReceiver, TId, amount, type, GetDateTimeNow(false), receiverId));

        }

        public void GenerateTransaction(string senderId, string receiverId, float amount, int type)
        {
            string TId = $"{GetBankId(receiverId)}{receiverId}{GetDateTimeNow(true)}";
            sqlHandler.ExecuiteNonQuery(String.Format(SqlQueries.InsertTransaction, TId, amount, type, GetDateTimeNow(false), senderId, receiverId));
        }

        public string DepositAmount(string accountId, float amount, string currencyName)
        {
            try
            {
                float currentBalance = GetBalance(accountId);
                float newBalance = currentBalance + amount * GetCurrencyValue(currencyName);
                sqlHandler.ExecuiteNonQuery(String.Format(SqlQueries.UpdateBalance, newBalance, accountId));
                GenerateTransaction(accountId, amount, 2);
                return GetName(accountId);
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
                sqlHandler.ExecuiteNonQuery(String.Format(SqlQueries.UpdateBalance, newBalance, accountId));
                GenerateTransaction(accountId, amount, 1);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool AuthenticateCustomer(string accountId, string pass)
        {
            var x = sqlHandler.ExecuiteScaler(String.Format(SqlQueries.AuthenticateCustomer, accountId, pass));
            return x.ToString() == "1" ? true : false;
        }

        public bool AuthenticateStaff(string accountId, string pass)
        {
            var x = sqlHandler.ExecuiteScaler(String.Format(SqlQueries.AuthenticateStaff, accountId, pass));
            return x.ToString() == "1" ? true : false;
        }

        public string UpdateCustomerName(string accountId, string newName)
        {
            sqlHandler.ExecuiteNonQuery(String.Format(String.Format(SqlQueries.UpdateName, newName, accountId)));
            return newName;
        }

        public string UpdateCustomerPassword(string accountId, string newPassword)
        {
            sqlHandler.ExecuiteNonQuery(String.Format(String.Format(SqlQueries.UpdatePassword, newPassword, accountId)));
            return newPassword;
        }

        public bool DeleteCustomerAccount(string accountId)
        {
            try
            {
                sqlHandler.ExecuiteNonQuery(String.Format(SqlQueries.DeleteCustomerAccount, accountId));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GetName(string accountId)
        {
            return sqlHandler.ExecuiteScaler(String.Format(SqlQueries.GetName, accountId)).ToString();
        }

        public float GetBalance(string accountId)
        {
            return float.Parse(sqlHandler.ExecuiteScaler(String.Format(SqlQueries.GetBalance, accountId)).ToString());
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

        public ConsoleTable GetTransactions(string accountId)
        {
            try
            {
                ConsoleTable table = sqlHandler.ExecuteReader(String.Format(SqlQueries.GetTransactions, accountId, accountId), new[] { "TransactionId", "SendersAccountId", "RecieversAccountId", "Type", "Amount", "Time" }, false);
                return table;
            }
            catch
            {
                return new ConsoleTable();
            }
        }

        public float UpdatesRTGS(float val, string bankId)
        {
            sqlHandler.ExecuiteNonQuery(String.Format(SqlQueries.UpdatesRTGS, val, bankId));
            return val;
        }

        public float UpdatesIMPS(float val, string bankId)
        {
            
            sqlHandler.ExecuiteNonQuery(String.Format(SqlQueries.UpdatesIMPS, val, bankId));
            return val;
        }

        public float UpdateoRTGS(float val, string bankId)
        {
            sqlHandler.ExecuiteNonQuery(String.Format(SqlQueries.UpdateoRTGS, val, bankId));
            return val;
        }

        public float UpdateoIMPS(float val, string bankId)
        {
            sqlHandler.ExecuiteNonQuery(String.Format(SqlQueries.UpdateoIMPS, val, bankId));
            return val;
        }

        public float GetBanksRTGSCharges(string bankId)
        {
            var x = sqlHandler.ExecuiteNonQuery(String.Format(SqlQueries.GetBanksRTGSCharges, bankId));
            return float.Parse(x.ToString());
        }
        public float GetBanksIMPSCharges(string bankId)
        {
            var x = sqlHandler.ExecuiteNonQuery(String.Format(SqlQueries.GetBanksRTGSCharges, bankId));
            return float.Parse(x.ToString());
        }
        public float GetBankoRTGSCharges(string bankId)
        {
            var x = sqlHandler.ExecuiteNonQuery(String.Format(SqlQueries.GetBanksRTGSCharges, bankId));
            return float.Parse(x.ToString());
        }
        public float GetBankoIMPSCharges(string bankId)
        {
            var x = sqlHandler.ExecuiteNonQuery(String.Format(SqlQueries.GetBanksRTGSCharges, bankId));
            return float.Parse(x.ToString());
        }

        public float GetBankProfits(string bankId)
        {
            var x = sqlHandler.ExecuiteScaler(String.Format(SqlQueries.GetBankProfits, bankId));
            return float.Parse(x.ToString());
        }
        public bool UpdateBankProfits(string bankId, float amount)
        {
            try
            {
                MySqlParameter p = new MySqlParameter();
                

                sqlHandler.ExecuiteScaler(String.Format(SqlQueries.UpdateBankProfits, amount, bankId));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GetBankId(string accountId)
        {
            var x = sqlHandler.ExecuiteScaler(String.Format(SqlQueries.GetBankId, accountId));
            return x.ToString();
        }

        public bool AddCurrency(string name, float value, string bankId)
        {
            try
            {
                sqlHandler.ExecuiteNonQuery(String.Format(SqlQueries.AddCurrency, name, bankId, value));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public float GetCurrencyValue(string currencyName)
        {
            var x = sqlHandler.ExecuiteScaler(String.Format(SqlQueries.GetCurrencyValue, currencyName));
            return float.Parse(x.ToString());
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
                    case (int)Transaction.TransactionType.Deposit:
                        if (receiversBalance - transactionAmount >= 0)
                        {
                            float newBalance = receiversBalance - transactionAmount;
                            UpdateBalance(receiversId, newBalance);
                        }
                        else
                        {
                            return false;
                        }
                        break;

                    case (int)Transaction.TransactionType.Withdraw:
                        UpdateBalance(receiversId, receiversBalance + transactionAmount);
                        break;

                    case (int)Transaction.TransactionType.Transfer:
                        string sendersId = GetTransactionSenderId(transactionId);

                        float newBal = receiversBalance - transactionAmount;
                        UpdateBalance(receiversId, newBal);

                        float sendersBalance = GetBalance(sendersId);
                        UpdateBalance(sendersId, sendersBalance + transactionAmount);
                        break;

                }
                sqlHandler.ExecuiteNonQuery(String.Format(SqlQueries.DeleteTransaction, transactionId));
                return true;
            }
            catch
            {
                return false;
            }

        }

        public float GetTransactionAmount(string transcationsId)
        {
            var x = sqlHandler.ExecuiteScaler(String.Format(SqlQueries.GetTransactionAmount, transcationsId));
            return float.Parse(x.ToString());
        }

        public int GetTransactionType(string transcationsId)
        {
            var x = sqlHandler.ExecuiteScaler(String.Format(SqlQueries.GetTransactionType, transcationsId));
            return int.Parse(x.ToString());
        }

        public string GetTransactionSenderId(string transactionId)
        {
            var x = sqlHandler.ExecuiteScaler(String.Format(SqlQueries.GetTransactionSenderId, transactionId));
            return x.ToString();
        }

        public string GetTransactionReceiverId(string transactionId)
        {
            var x = sqlHandler.ExecuiteScaler(String.Format(SqlQueries.GetTransactionReceiverId, transactionId));
            return x.ToString();
        }
    }
}