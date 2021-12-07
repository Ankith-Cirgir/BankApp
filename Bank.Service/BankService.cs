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

            //CreateStaffAccount("admin", "admin", "Mon09112021");
            AddCurrency("INR", 1, "Mon09112021");
        }

        public string GetDateTimeNow(bool forId)
        {
            return forId ? DateTime.Now.ToString("ddMMyyyyHHmmss") : DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        public string AddBank(string name, float sRTGS, float sIMPS, float oRTGS, float oIMPS)
        {
            string bankId = $"{name.Substring(0, 3)}{GetDateTimeNow(true)}";
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@BankId",bankId));
            parameterList.Add(new MySqlParameter("@BankName", name));
            parameterList.Add(new MySqlParameter("@sRTGSCharge", sRTGS));
            parameterList.Add(new MySqlParameter("@sIMPSCharge", sIMPS));
            parameterList.Add(new MySqlParameter("@oRTGSCharge", oRTGS));
            parameterList.Add(new MySqlParameter("@oIMPSCharge", oIMPS));
            parameterList.Add(new MySqlParameter("@Profits", 0));
            sqlHandler.ExecuiteNonQuery(SqlQueries.InsertIntoBanksTable, parameterList);
            return bankId;
        }

        public string AddBank(string name)
        {
            string bankId = $"{name.Substring(0, 3)}{GetDateTimeNow(true)}";
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@BankId", bankId));
            parameterList.Add(new MySqlParameter("@BankName", name));
            parameterList.Add(new MySqlParameter("@sRTGSCharge", 0));
            parameterList.Add(new MySqlParameter("@sIMPSCharge", 5));
            parameterList.Add(new MySqlParameter("@oRTGSCharge", 2));
            parameterList.Add(new MySqlParameter("@oIMPSCharge", 6));
            parameterList.Add(new MySqlParameter("@Profits", 0)); 
            sqlHandler.ExecuiteNonQuery(SqlQueries.InsertIntoBanksTable, parameterList);

            return bankId;
        }

        public string CreateCustomerAccount(string name, string pass, string bankId)
        {
            string accountId = $"{name.Substring(0, 3)}{GetDateTimeNow(true)}";

            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@AccountId", accountId));
            parameterList.Add(new MySqlParameter("@BankId", bankId));
            parameterList.Add(new MySqlParameter("@Password", pass));
            parameterList.Add(new MySqlParameter("@Name", name));
            parameterList.Add(new MySqlParameter("@Balance", 0));

            sqlHandler.ExecuiteNonQuery(SqlQueries.InsertIntoCustomersTable, parameterList);
            return accountId;
        }

        public string CreateStaffAccount(string name, string pass, string bankId)
        {
            string accountId = $"{name.Substring(0, 3)}{GetDateTimeNow(true)}";
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@BankId", bankId));
            parameterList.Add(new MySqlParameter("@Name", name));
            parameterList.Add(new MySqlParameter("@Password", pass));


            sqlHandler.ExecuiteNonQuery(SqlQueries.InsertIntoStaffsTable, parameterList);
            return accountId;
        }

        public bool UpdateBalance(string accountId, float newBalance)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@newBalance", newBalance));
            parameterList.Add(new MySqlParameter("@AccountId", accountId));

            sqlHandler.ExecuiteNonQuery(SqlQueries.UpdateBalance, parameterList);
            return true;
        }

        public void GenerateTransaction(string receiverId, float amount, int type)
        {
            string TId = $"{GetBankId(receiverId)}{receiverId}{GetDateTimeNow(true)}";
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@TransactionId", TId));
            parameterList.Add(new MySqlParameter("@Amount", amount));
            parameterList.Add(new MySqlParameter("@Type", type));
            parameterList.Add(new MySqlParameter("@Time", GetDateTimeNow(false)));
            parameterList.Add(new MySqlParameter("@ReceiverId", receiverId));

            sqlHandler.ExecuiteNonQuery(SqlQueries.InsertTransactionReceiver, parameterList);

        }

        public void GenerateTransaction(string senderId, string receiverId, float amount, int type)
        {
            string TId = $"{GetBankId(receiverId)}{receiverId}{GetDateTimeNow(true)}";
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@TransactionId", TId));
            parameterList.Add(new MySqlParameter("@Amount", amount));
            parameterList.Add(new MySqlParameter("@Type", type));
            parameterList.Add(new MySqlParameter("@Time", GetDateTimeNow(false)));
            parameterList.Add(new MySqlParameter("@ReceiverId", receiverId));
            parameterList.Add(new MySqlParameter("@SenderId", senderId));



            sqlHandler.ExecuiteNonQuery(SqlQueries.InsertTransaction, parameterList);
        }

        public string DepositAmount(string accountId, float amount, string currencyName)
        {
            try
            {
                float currentBalance = GetBalance(accountId);
                float newBalance = currentBalance + amount * GetCurrencyValue(currencyName);

                UpdateBalance(accountId, newBalance);
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
                UpdateBalance(accountId, newBalance);
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
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@AccountId", accountId));
            parameterList.Add(new MySqlParameter("@Password", pass));

            var x = sqlHandler.ExecuiteScaler(SqlQueries.AuthenticateCustomer, parameterList);
            return x.ToString() == "1";
        }

        public bool AuthenticateStaff(string accountId, string pass)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@AccountId", accountId));
            parameterList.Add(new MySqlParameter("@Password", pass));

            var x = sqlHandler.ExecuiteScaler(SqlQueries.AuthenticateStaff, parameterList);
            return x.ToString() == "1";
        }

        public string UpdateCustomerName(string accountId, string newName)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@AccountId", accountId));
            parameterList.Add(new MySqlParameter("@newName", newName));

            sqlHandler.ExecuiteNonQuery(SqlQueries.UpdateName, parameterList);
            return newName;
        }

        public string UpdateCustomerPassword(string accountId, string newPassword)
        {

            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@AccountId", accountId));
            parameterList.Add(new MySqlParameter("@newName", newPassword));


            sqlHandler.ExecuiteNonQuery(SqlQueries.UpdatePassword, parameterList);
            return newPassword;
        }

        public bool DeleteCustomerAccount(string accountId)
        {
            try
            {
                List<MySqlParameter> parameterList = new List<MySqlParameter>();
                parameterList.Add(new MySqlParameter("@AccountId", accountId));

                sqlHandler.ExecuiteNonQuery(SqlQueries.DeleteCustomerAccount, parameterList);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GetName(string accountId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@AccountId", accountId));

            return sqlHandler.ExecuiteScaler(SqlQueries.GetName, parameterList).ToString();
        }

        public float GetBalance(string accountId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@AccountId", accountId));

            return float.Parse(sqlHandler.ExecuiteScaler(SqlQueries.GetBalance, parameterList).ToString());
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
                List<MySqlParameter> parameterList = new List<MySqlParameter>();
                parameterList.Add(new MySqlParameter("@ReceiverId", accountId));
                parameterList.Add(new MySqlParameter("@SenderId", accountId));

                ConsoleTable table = sqlHandler.ExecuteReader(SqlQueries.GetTransactions, parameterList, new[] { "TransactionId", "SendersAccountId", "RecieversAccountId", "Type", "Amount", "Time" }, false);
                return table;
            }
            catch
            {
                return new ConsoleTable();
            }
        }

        public float UpdatesRTGS(float val, string bankId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@sRTGSCharge", val));
            parameterList.Add(new MySqlParameter("@BankId", bankId));


            sqlHandler.ExecuiteNonQuery(SqlQueries.UpdatesRTGS, parameterList);
            return val;
        }

        public float UpdatesIMPS(float val, string bankId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@sIMPSCharge", val));

            sqlHandler.ExecuiteNonQuery(SqlQueries.UpdatesIMPS, parameterList);
            return val;
        }

        public float UpdateoRTGS(float val, string bankId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@oRTGSCharge", val));
            parameterList.Add(new MySqlParameter("@BankId", bankId));

            sqlHandler.ExecuiteNonQuery(SqlQueries.UpdateoRTGS, parameterList);
            return val;
        }

        public float UpdateoIMPS(float val, string bankId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@oIMPSCharge", val));
            parameterList.Add(new MySqlParameter("@BankId", bankId));

            sqlHandler.ExecuiteNonQuery(SqlQueries.UpdateoIMPS, parameterList);
            return val;
        }

        public float GetBanksRTGSCharges(string bankId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@BankId", bankId));

            var x = sqlHandler.ExecuiteNonQuery(SqlQueries.GetBanksRTGSCharges, parameterList);
            return float.Parse(x.ToString());
        }
        public float GetBanksIMPSCharges(string bankId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@BankId", bankId));

            var x = sqlHandler.ExecuiteNonQuery(SqlQueries.GetBanksRTGSCharges, parameterList);
            return float.Parse(x.ToString());
        }
        public float GetBankoRTGSCharges(string bankId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@BankId", bankId));

            var x = sqlHandler.ExecuiteNonQuery(SqlQueries.GetBanksRTGSCharges, parameterList);
            return float.Parse(x.ToString());
        }
        public float GetBankoIMPSCharges(string bankId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@BankId", bankId));

            var x = sqlHandler.ExecuiteNonQuery(SqlQueries.GetBanksRTGSCharges, parameterList);
            return float.Parse(x.ToString());
        }

        public float GetBankProfits(string bankId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@BankId", bankId));

            var x = sqlHandler.ExecuiteScaler(SqlQueries.GetBankProfits, parameterList);
            return float.Parse(x.ToString());
        }
        public bool UpdateBankProfits(string bankId, float amount)
        {
            try
            {
                List<MySqlParameter> parameterList = new List<MySqlParameter>();
                parameterList.Add(new MySqlParameter("@newProfits", amount));
                parameterList.Add(new MySqlParameter("@BankId", bankId));

                sqlHandler.ExecuiteScaler(SqlQueries.UpdateBankProfits, parameterList);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GetBankId(string accountId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@AccountId", accountId));
            var x = sqlHandler.ExecuiteScaler(SqlQueries.GetBankId, parameterList);
            return x.ToString();
        }

        public bool AddCurrency(string name, float value, string bankId)
        {
            try
            {
                List<MySqlParameter> parameterList = new List<MySqlParameter>();
                parameterList.Add(new MySqlParameter("@BankId", bankId));
                parameterList.Add(new MySqlParameter("@Currency", name));
                parameterList.Add(new MySqlParameter("@Value", value));

                sqlHandler.ExecuiteNonQuery(SqlQueries.AddCurrency, parameterList);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public float GetCurrencyValue(string currencyName)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@CurrencyName", currencyName));

            var x = sqlHandler.ExecuiteScaler(SqlQueries.GetCurrencyValue, parameterList);
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
                List<MySqlParameter> parameterList = new List<MySqlParameter>();
                parameterList.Add(new MySqlParameter("@TransactionId", transactionId));

                sqlHandler.ExecuiteNonQuery(SqlQueries.DeleteTransaction, parameterList);
                return true;
            }
            catch
            {
                return false;
            }

        }

        public float GetTransactionAmount(string transactionId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@TransactionId", transactionId));

            var x = sqlHandler.ExecuiteScaler(SqlQueries.GetTransactionAmount, parameterList);
            return float.Parse(x.ToString());
        }

        public int GetTransactionType(string transactionId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@TransactionId", transactionId));

            var x = sqlHandler.ExecuiteScaler(SqlQueries.GetTransactionType, parameterList);
            return int.Parse(x.ToString());
        }

        public string GetTransactionSenderId(string transactionId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@TransactionId", transactionId));

            var x = sqlHandler.ExecuiteScaler(SqlQueries.GetTransactionSenderId, parameterList);
            return x.ToString();
        }

        public string GetTransactionReceiverId(string transactionId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@TransactionId", transactionId));

            var x = sqlHandler.ExecuiteScaler(SqlQueries.GetTransactionReceiverId, parameterList);
            return x.ToString();
        }
    }
}